Option Strict On

Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Configuration
Imports System
Imports System.Drawing
' Hexor UA 
'   http://www.dev-point.com/vb
' <U A C O D E R>
Namespace Encryption

#Region " OSAMA/UACODER "

    ''' <summary>
    ''' Symmetric encryption uses a single key to encrypt and decrypt. 
    ''' Both parties (encryptor and decryptor) must share the same secret key.
    ''' </summary>
    Public Class Symmetric

        Private Const _DefaultIntializationVector As String = "%1Az=-@qT"
        Private Const _BufferSize As Integer = 2048

        Public Enum Provider
            ''' <summary>
            ''' The Data Encryption Standard provider supports a 64 bit key only
            ''' </summary>
            DES
            ''' <summary>
            ''' The Rivest Cipher 2 provider supports keys ranging from 40 to 128 bits, default is 128 bits
            ''' </summary>
            RC2
            ''' <summary>
            ''' The Rijndael (also known as AES) provider supports keys of 128, 192, or 256 bits with a default of 256 bits
            ''' </summary>
            Rijndael
            ''' <summary>
            ''' The TripleDES provider (also known as 3DES) supports keys of 128 or 192 bits with a default of 192 bits
            ''' </summary>
            TripleDES
        End Enum

        '        Private _data As Data
        Private _key As Data
        Private _iv As Data
        Private _crypto As SymmetricAlgorithm
        '        Private _EncryptedBytes As Byte()
        '        Private _UseDefaultInitializationVector As Boolean

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Instantiates a new symmetric encryption object using the specified provider.
        ''' </summary>
        Public Sub New(ByVal provider As Provider, Optional ByVal useDefaultInitializationVector As Boolean = True)
            Select Case provider
                Case provider.DES
                    _crypto = New DESCryptoServiceProvider
                Case provider.RC2
                    _crypto = New RC2CryptoServiceProvider
                Case provider.Rijndael
                    _crypto = New RijndaelManaged
                Case provider.TripleDES
                    _crypto = New TripleDESCryptoServiceProvider
            End Select

            '-- make sure key and IV are always set, no matter what
            Me.Key = RandomKey()
            If useDefaultInitializationVector Then
                Me.IntializationVector = New Data(_DefaultIntializationVector)
            Else
                Me.IntializationVector = RandomInitializationVector()
            End If
        End Sub

        '        ''' <summary>
        '        ''' Key size in bytes. We use the default key size for any given provider; if you 
        '        ''' want to force a specific key size, set this property
        '        ''' </summary>
        '        Public Property KeySizeBytes() As Integer
        '            Get
        '                Return _crypto.KeySize \ 8
        '            End Get
        '            Set(ByVal Value As Integer)
        '                _crypto.KeySize = Value * 8
        '                _key.MaxBytes = Value
        '            End Set
        '        End Property

        '        ''' <summary>
        '        ''' Key size in bits. We use the default key size for any given provider; if you 
        '        ''' want to force a specific key size, set this property
        '        ''' </summary>
        '        Public Property KeySizeBits() As Integer
        '            Get
        '                Return _crypto.KeySize
        '            End Get
        '            Set(ByVal Value As Integer)
        '                _crypto.KeySize = Value
        '                _key.MaxBits = Value
        '            End Set
        '        End Property

        ''' <summary>
        ''' The key used to encrypt/decrypt data
        ''' </summary>
        Public Property Key() As Data
            Get
                Return _key
            End Get
            Set(ByVal Value As Data)
                _key = Value
                _key.MaxBytes = _crypto.LegalKeySizes(0).MaxSize \ 8
                _key.MinBytes = _crypto.LegalKeySizes(0).MinSize \ 8
                _key.StepBytes = _crypto.LegalKeySizes(0).SkipSize \ 8
            End Set
        End Property

        ''' <summary>
        ''' Using the default Cipher Block Chaining (CBC) mode, all data blocks are processed using
        ''' the value derived from the previous block; the first data block has no previous data block
        ''' to use, so it needs an InitializationVector to feed the first block
        ''' </summary>
        Public Property IntializationVector() As Data
            Get
                Return _iv
            End Get
            Set(ByVal Value As Data)
                _iv = Value
                _iv.MaxBytes = _crypto.BlockSize \ 8
                _iv.MinBytes = _crypto.BlockSize \ 8
            End Set
        End Property

        ''' <summary>
        ''' generates a random Initialization Vector, if one was not provided
        ''' </summary>
        Public Function RandomInitializationVector() As Data
            _crypto.GenerateIV()
            Dim d As New Data(_crypto.IV)
            Return d
        End Function

        ''' <summary>
        ''' generates a random Key, if one was not provided
        ''' </summary>
        Public Function RandomKey() As Data
            _crypto.GenerateKey()
            Dim d As New Data(_crypto.Key)
            Return d
        End Function

        ''' <summary>
        ''' Ensures that _crypto object has valid Key and IV
        ''' prior to any attempt to encrypt/decrypt anything
        ''' </summary>
        Private Sub ValidateKeyAndIv(ByVal isEncrypting As Boolean)
            If _key.IsEmpty Then
                If isEncrypting Then
                    _key = RandomKey()
                Else
                    Throw New CryptographicException("No key was provided for the decryption operation!")
                End If
            End If
            If _iv.IsEmpty Then
                If isEncrypting Then
                    _iv = RandomInitializationVector()
                Else
                    Throw New CryptographicException("No initialization vector was provided for the decryption operation!")
                End If
            End If
            _crypto.Key = _key.Bytes
            _crypto.IV = _iv.Bytes
        End Sub

        ''' <summary>
        ''' Encrypts the specified Data using provided key
        ''' </summary>
        Public Function Encrypt(ByVal d As Data, ByVal key As Data) As Data
            Me.Key = key
            Return Encrypt(d)
        End Function

        ''' <summary>
        ''' Encrypts the specified Data using preset key and preset initialization vector
        ''' </summary>
        Public Function Encrypt(ByVal d As Data) As Data
            Dim ms As New IO.MemoryStream

            ValidateKeyAndIv(True)

            Dim cs As New CryptoStream(ms, _crypto.CreateEncryptor(), CryptoStreamMode.Write)
            cs.Write(d.Bytes, 0, d.Bytes.Length)
            cs.Close()
            ms.Close()

            Return New Data(ms.ToArray)
        End Function

        ''' <summary>
        ''' Encrypts the stream to memory using provided key and provided initialization vector
        ''' </summary>
        Public Function Encrypt(ByVal s As Stream, ByVal key As Data, ByVal iv As Data) As Data
            Me.IntializationVector = iv
            Me.Key = key
            Return Encrypt(s)
        End Function

        ''' <summary>
        ''' Encrypts the stream to memory using specified key
        ''' </summary>
        Public Function Encrypt(ByVal s As Stream, ByVal key As Data) As Data
            Me.Key = key
            Return Encrypt(s)
        End Function

        ''' <summary>
        ''' Encrypts the specified stream to memory using preset key and preset initialization vector
        ''' </summary>
        Public Function Encrypt(ByVal s As Stream) As Data
            Dim ms As New IO.MemoryStream
            Dim b(_BufferSize) As Byte
            Dim i As Integer

            ValidateKeyAndIv(True)

            Dim cs As New CryptoStream(ms, _crypto.CreateEncryptor(), CryptoStreamMode.Write)
            i = s.Read(b, 0, _BufferSize)
            Do While i > 0
                cs.Write(b, 0, i)
                i = s.Read(b, 0, _BufferSize)
            Loop

            cs.Close()
            ms.Close()

            Return New Data(ms.ToArray)
        End Function

        ''' <summary>
        ''' Decrypts the specified data using provided key and preset initialization vector
        ''' </summary>
        Public Function Decrypt(ByVal encryptedData As Data, ByVal key As Data) As Data
            Me.Key = key
            Return Decrypt(encryptedData)
        End Function

        ''' <summary>
        ''' Decrypts the specified stream using provided key and preset initialization vector
        ''' </summary>
        Public Function Decrypt(ByVal encryptedStream As Stream, ByVal key As Data) As Data
            Me.Key = key
            Return Decrypt(encryptedStream)
        End Function

        ''' <summary>
        ''' Decrypts the specified stream using preset key and preset initialization vector
        ''' </summary>
        Public Function Decrypt(ByVal encryptedStream As Stream) As Data
            Dim ms As New System.IO.MemoryStream
            Dim b(_BufferSize) As Byte

            ValidateKeyAndIv(False)
            Dim cs As New CryptoStream(encryptedStream, _
                _crypto.CreateDecryptor(), CryptoStreamMode.Read)

            Dim i As Integer
            i = cs.Read(b, 0, _BufferSize)

            Do While i > 0
                ms.Write(b, 0, i)
                i = cs.Read(b, 0, _BufferSize)
            Loop
            cs.Close()
            ms.Close()

            Return New Data(ms.ToArray)
        End Function

        ''' <summary>
        ''' Decrypts the specified data using preset key and preset initialization vector
        ''' </summary>
        Public Function Decrypt(ByVal encryptedData As Data) As Data
            Try
                Dim ms As New System.IO.MemoryStream(encryptedData.Bytes, 0, encryptedData.Bytes.Length)
                Dim b() As Byte = New Byte(encryptedData.Bytes.Length - 1) {}

                ValidateKeyAndIv(False)
                Dim cs As New CryptoStream(ms, _crypto.CreateDecryptor(), CryptoStreamMode.Read)

                Try
                    cs.Read(b, 0, encryptedData.Bytes.Length - 1)
                Catch ex As CryptographicException
                    Throw New CryptographicException("Unable to decrypt data. The provided key may be invalid.", ex)
                Finally
                    cs.Close()
                End Try
                Return New Data(b)
            Catch
                Return Nothing
            End Try
        End Function

    End Class

#End Region

#Region "  Data"

    ''' <summary>
    ''' represents Hex, Byte, Base64, or String data to encrypt/decrypt;
    ''' use the .Text property to set/get a string representation 
    ''' use the .Hex property to set/get a string-based Hexadecimal representation 
    ''' use the .Base64 to set/get a string-based Base64 representation 
    ''' </summary>
    Public Class Data
        Private _b As Byte()
        Private _MaxBytes As Integer = 0
        Private _MinBytes As Integer = 0
        Private _StepBytes As Integer = 0

        ''' <summary>
        ''' Determines the default text encoding across ALL Data instances
        ''' </summary>
        Public Shared DefaultEncoding As System.Text.Encoding = System.Text.Encoding.GetEncoding("Windows-1252")

        ''' <summary>
        ''' Determines the default text encoding for this Data instance
        ''' </summary>
        Public Encoding As System.Text.Encoding = DefaultEncoding

        ''' <summary>
        ''' Creates new, empty encryption data
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Creates new encryption data with the specified byte array
        ''' </summary>
        Public Sub New(ByVal b As Byte())
            _b = b
        End Sub

        ''' <summary>
        ''' Creates new encryption data with the specified string; 
        ''' will be converted to byte array using default encoding
        ''' </summary>
        Public Sub New(ByVal s As String)
            Me.Text = s
        End Sub

        ''' <summary>
        ''' Creates new encryption data using the specified string and the 
        ''' specified encoding to convert the string to a byte array.
        ''' </summary>
        Public Sub New(ByVal s As String, ByVal encoding As System.Text.Encoding)
            Me.Encoding = encoding
            Me.Text = s
        End Sub

        ''' <summary>
        ''' returns true if no data is present
        ''' </summary>
        Public ReadOnly Property IsEmpty() As Boolean
            Get
                If _b Is Nothing Then
                    Return True
                End If
                If _b.Length = 0 Then
                    Return True
                End If
                Return False
            End Get
        End Property

        ''' <summary>
        ''' allowed step interval, in bytes, for this data; if 0, no limit
        ''' </summary>
        Public Property StepBytes() As Integer
            Get
                Return _StepBytes
            End Get
            Set(ByVal Value As Integer)
                _StepBytes = Value
            End Set
        End Property

        '        ''' <summary>
        '        ''' allowed step interval, in bits, for this data; if 0, no limit
        '        ''' </summary>
        '        Public Property StepBits() As Integer
        '            Get
        '                Return _StepBytes * 8
        '            End Get
        '            Set(ByVal Value As Integer)
        '                _StepBytes = Value \ 8
        '            End Set
        '        End Property

        ''' <summary>
        ''' minimum number of bytes allowed for this data; if 0, no limit
        ''' </summary>
        Public Property MinBytes() As Integer
            Get
                Return _MinBytes
            End Get
            Set(ByVal Value As Integer)
                _MinBytes = Value
            End Set
        End Property

        '        ''' <summary>
        '        ''' minimum number of bits allowed for this data; if 0, no limit
        '        ''' </summary>
        '        Public Property MinBits() As Integer
        '            Get
        '                Return _MinBytes * 8
        '            End Get
        '            Set(ByVal Value As Integer)
        '                _MinBytes = Value \ 8
        '            End Set
        '        End Property

        ''' <summary>
        ''' maximum number of bytes allowed for this data; if 0, no limit
        ''' </summary>
        Public Property MaxBytes() As Integer
            Get
                Return _MaxBytes
            End Get
            Set(ByVal Value As Integer)
                _MaxBytes = Value
            End Set
        End Property

        '        ''' <summary>
        '        ''' maximum number of bits allowed for this data; if 0, no limit
        '        ''' </summary>
        '        Public Property MaxBits() As Integer
        '            Get
        '                Return _MaxBytes * 8
        '            End Get
        '            Set(ByVal Value As Integer)
        '                _MaxBytes = Value \ 8
        '            End Set
        '        End Property

        ''' <summary>
        ''' Returns the byte representation of the data; 
        ''' This will be padded to MinBytes and trimmed to MaxBytes as necessary!
        ''' </summary>
        Public Property Bytes() As Byte()
            Get
                If _MaxBytes > 0 Then
                    If _b.Length > _MaxBytes Then
                        Dim b(_MaxBytes - 1) As Byte
                        Array.Copy(_b, b, b.Length)
                        _b = b
                    End If
                End If
                If _MinBytes > 0 Then
                    If _b.Length < _MinBytes Then
                        Dim b(_MinBytes - 1) As Byte
                        Array.Copy(_b, b, _b.Length)
                        _b = b
                    End If
                End If
                Return _b
            End Get
            Set(ByVal Value As Byte())
                _b = Value
            End Set
        End Property

        ''' <summary>
        ''' Sets or returns text representation of bytes using the default text encoding
        ''' </summary>
        Public Property Text() As String
            Get
                If _b Is Nothing Then
                    Return ""
                Else
                    '-- need to handle nulls here; oddly, C# will happily convert
                    '-- nulls into the string whereas VB stops converting at the
                    '-- first null!
                    Dim i As Integer = Array.IndexOf(_b, CType(0, Byte))
                    If i >= 0 Then
                        Return Me.Encoding.GetString(_b, 0, i)
                    Else
                        Return Me.Encoding.GetString(_b)
                    End If
                End If
            End Get
            Set(ByVal Value As String)
                _b = Me.Encoding.GetBytes(Value)
            End Set
        End Property

        ''' <summary>
        ''' Sets or returns Hex string representation of this data
        ''' </summary>
        Public Property Hex() As String
            Get
                Return Utils.ToHex(_b)
            End Get
            Set(ByVal Value As String)
                _b = Utils.FromHex(Value)
            End Set
        End Property

        '        ''' <summary>
        '        ''' Sets or returns Base64 string representation of this data
        '        ''' </summary>
        '        Public Property Base64() As String
        '            Get
        '                Return Utils.ToBase64(_b)
        '            End Get
        '            Set(ByVal Value As String)
        '                _b = Utils.FromBase64(Value)
        '            End Set
        '        End Property

        '        ''' <summary>
        '        ''' Returns text representation of bytes using the default text encoding
        '        ''' </summary>
        '        Public Shadows Function ToString() As String
        '            Return Me.Text
        '        End Function

        '        ''' <summary>
        '        ''' returns Base64 string representation of this data
        '        ''' </summary>
        '        Public Function ToBase64() As String
        '            Return Me.Base64
        '        End Function

        ''' <summary>
        ''' returns Hex string representation of this data
        ''' </summary>
        Public Function ToHex() As String
            Return Me.Hex
        End Function

    End Class

#End Region

#Region "  Utils"

    ''' <summary>
    ''' Friend class for shared utility methods used by multiple Encryption classes
    ''' </summary>
    Friend Class Utils

        ''' <summary>
        ''' converts an array of bytes to a string Hex representation
        ''' </summary>
        Friend Shared Function ToHex(ByVal ba() As Byte) As String
            If ba Is Nothing OrElse ba.Length = 0 Then
                Return ""
            End If
            Const HexFormat As String = "{0:X2}"
            Dim sb As New StringBuilder
            For Each b As Byte In ba
                sb.Append(String.Format(HexFormat, b))
            Next
            Return sb.ToString
        End Function

        ''' <summary>
        ''' converts from a string Hex representation to an array of bytes
        ''' </summary>
        Friend Shared Function FromHex(ByVal hexEncoded As String) As Byte()
            If hexEncoded Is Nothing OrElse hexEncoded.Length = 0 Then
                Return Nothing
            End If
            Try
                Dim l As Integer = Convert.ToInt32(hexEncoded.Length / 2)
                Dim b(l - 1) As Byte
                For i As Integer = 0 To l - 1
                    b(i) = Convert.ToByte(hexEncoded.Substring(i * 2, 2), 16)
                Next
                Return b
            Catch ex As Exception
                Throw New System.FormatException("The provided string does not appear to be Hex encoded:" & _
                    Environment.NewLine & hexEncoded & Environment.NewLine, ex)
            End Try
        End Function
    End Class
#End Region
End Namespace