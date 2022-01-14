Public Class Form1
#Region " - Generate key - "
    ' يقوم هذا الكود بتوليد مفاتيح للمستخدمين مشفرة 
    Private Sub txtGKClientName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtGKClientName.TextChanged
        GenerateKey(txtPassPhrase.Text, txtGKClientName.Text)
    End Sub

    Private Sub txtPassPhrase_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPassPhrase.TextChanged
        GenerateKey(txtPassPhrase.Text, txtGKClientName.Text)
    End Sub

    Private Sub GenerateKey(ByVal pPassPhrase As String, ByVal pClientName As String)
        Dim strRegkey As String = Encrypt(pPassPhrase, pClientName.ToUpper)
        If strRegkey.Length > 0 Then
            strRegkey = strRegkey.Remove(16, (strRegkey.Length - 16))
            strRegkey = strRegkey.Insert(4, "-")
            strRegkey = strRegkey.Insert(8, "-")
            strRegkey = strRegkey.Insert(12, "-")
        End If
        txtGeneratedKey.Text = strRegkey
    End Sub

#End Region
#Region " - Common methods - "

    Private Function Encrypt(ByRef pPassPhrase As String, ByVal pTextToEncrypt As String) As String
        If pPassPhrase.Length > 16 Then
            'هذه قيود على ألية التشفير
            pPassPhrase = pPassPhrase.Substring(0, 16)
        End If

        If pTextToEncrypt.Trim.Length = 0 Then
            'وهذا لكون نص تشفير لم يحدد
            Return String.Empty
        End If

        Dim skey As New Encryption.Data(pPassPhrase)
        Dim sym As New Encryption.Symmetric(Encryption.Symmetric.Provider.Rijndael)
        Dim objEncryptedData As Encryption.Data
        objEncryptedData = sym.Encrypt(New Encryption.Data(pTextToEncrypt), skey)
        Return objEncryptedData.ToHex
    End Function

    Private Function Decrypt(ByRef pPassPhrase As String, ByVal pHexStream As String) As String
        'هذي معروفة لفك التشفير :P
        ' محبكم أسامه
        Try
            Dim objSym As New Encryption.Symmetric(Encryption.Symmetric.Provider.Rijndael)
            Dim encryptedData As New Encryption.Data
            encryptedData.Hex = pHexStream
            Dim decryptedData As Encryption.Data
            decryptedData = objSym.Decrypt(encryptedData, New Encryption.Data(pPassPhrase))
            Return decryptedData.Text
        Catch
            Return Nothing
        End Try
    End Function
#End Region
End Class
