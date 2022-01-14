Option Strict On
Public Class Form1
    Private kstrRegSubKeyName As String = "Uacoder1\\Uacoder-Trial1" 'اسم لمفتاح فرعي لاستعادة معلومات التسجيل
    Private mintUsedTrialDays As Integer
    Private mintTrialPeriod As Integer = 30 'مدة ترخيص البرنامج
    Private mblnInTrial As Boolean = True
    Private mblnFullVersion As Boolean = False
    Private Structure CurrentDate
        Dim Day As String ' تعريف ليوم من نوع إسترينق
        Dim Month As String ' تعريف لشهر من نوع إسترينق
        Dim Year As String ' تعريف لسنة من نوع إسترينق
    End Structure
    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Dim oReg As Microsoft.Win32.RegistryKey
        oReg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", True)
        oReg = oReg.CreateSubKey(kstrRegSubKeyName)
        oReg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\" & kstrRegSubKeyName)
        Dim strOldDay As String = oReg.GetValue("UserSettings", "").ToString
        Dim strOldMonth As String = oReg.GetValue("operatingsystem", "").ToString
        Dim strOldYear As String = oReg.GetValue("GUID", "").ToString
        Dim strRegName As String = oReg.GetValue("USERID", "").ToString
        Dim strRegCode As String = oReg.GetValue("LOCALPATH", "").ToString
        Dim strCompID As String = oReg.GetValue("CompID", "").ToString
        Dim strTrialDone As String = oReg.GetValue("Enable", "").ToString
        oReg.Close()

        'إذ سيتم تلقائياً إنشاء المفتاح، ثم صناعتها  لهم
        If strOldDay = "" Then
            CreateRegKeys(txtPassPhrase.Text)
        End If

        'إذا كانت المفاتيح مشفرة سوف يقوم بفك التشفير لهم
        'If EncryptKeys = True Then
        strOldDay = Decrypt(txtPassPhrase.Text, strOldDay)
        strOldMonth = Decrypt(txtPassPhrase.Text, strOldMonth)
        strOldYear = Decrypt(txtPassPhrase.Text, strOldYear)
        'End If

        'تعريف المتغيرات
        mintUsedTrialDays = DiffDate(strOldDay, strOldMonth, strOldYear)

        'ملئ شريط التقدم من النسخة التجريبية
        lblApplicationStatus.Text = DisplayApplicationStatus(DiffDate(strOldDay, strOldMonth, strOldYear), mintTrialPeriod)

        'تعطيل زر المتابعة في البرنامج إذا كانت النسخة التجريبية إنتهت
        If DiffDate(strOldDay, strOldMonth, strOldYear) > mintTrialPeriod Then
            'unregbutton.Enabled = False
            mblnInTrial = False
            oReg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", True)
            oReg = oReg.CreateSubKey(kstrRegSubKeyName)
            oReg.SetValue("Enable", "1")
            oReg.Close()
        End If

        'إذا كان التاريخ أقدم سوف يقوم البرنامج بالتعطيل
        If strOldMonth = "" Then
        Else
            Dim dtmOldDate As Date = New Date(Convert.ToInt32(strOldYear), Convert.ToInt32(strOldMonth), Convert.ToInt32(strOldDay))
            If Date.Compare(DateTime.Now, dtmOldDate) < 0 Then
                'unregbutton.Enabled = False
                mblnInTrial = False
                oReg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", True)
                oReg = oReg.CreateSubKey(kstrRegSubKeyName)
                oReg.SetValue("Enable", "1")
                oReg.Close()
            End If
        End If


        'إذا لم يتم تفعيل النسخة التجريبية يقوم بالتعطيل
        If strTrialDone = "1" Then
            'unregbutton.Enabled = False
            mblnInTrial = False
            lblApplicationStatus.Text = "The system clock has been manually changed, and the application has been locked out to prevent unauthorized access!"
        End If

        'معرفة ما إذا تم تسجيل المستخدم بالفعل، وإذا كان الأمر كذلك سيتم إعادة عملية المعلومات ومعرفة ما اذا كان الكمبيوتر هو وكل شيء سليم
        If strRegName = "" Then
        Else
            Dim strRN As String = Decrypt(txtPassPhrase.Text, strRegName)
            Dim strRC As String = Decrypt(txtPassPhrase.Text, strRegCode)
            Dim UserName As String = strRegName
            UserName = UserName.Remove(16, (UserName.Length - 16))
            If UserName = Decrypt(txtPassPhrase.Text, strRegCode) Then
                If Encrypt(txtPassPhrase.Text, cHardware.GetMotherBoardID.Trim.ToString) = strCompID Then
                    mblnInTrial = False
                    mblnFullVersion = True

                    strRC = strRC.Insert(4, "-")
                    strRC = strRC.Insert(8, "-")
                    strRC = strRC.Insert(12, "-")
                    lblApplicationStatus.ForeColor = Color.Green
                    lblApplicationStatus.Text = "Licensed Version To " + strRN + " With The Key " + strRC
                    oReg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", True)
                    oReg = oReg.CreateSubKey(kstrRegSubKeyName)
                    oReg.SetValue("Enable", "")
                    oReg.Close()

                End If
                txtKeyToValidate.Enabled = False
                txtVKClientName.Enabled = False
                txtPassPhrase.Enabled = False
                btnTestKey.Enabled = False
                Form2.Show()
                Me.Close()
            End If
        End If
    End Sub
    Sub Protect()

        Dim TargetProcess1() As Process = Process.GetProcessesByName("Fiddler")
        Dim TargetProcess2() As Process = Process.GetProcessesByName("SimpleAssemblyExplorer ")
        Dim TargetProcess3() As Process = Process.GetProcessesByName("Reflector")
        Dim TargetProcess4() As Process = Process.GetProcessesByName("HxD")
        Dim TargetProcess5() As Process = Process.GetProcessesByName("Ollydb")

        If Not TargetProcess1.Length = 0 Then
            TargetProcess1(0).Kill()
            MsgBox("Fiddler Detected!", MsgBoxStyle.Critical, "WTF?!")
            Me.Close()
        End If

        If Not TargetProcess2.Length = 0 Then
            TargetProcess2(0).Kill()
            MsgBox("SimpleAssemblyExplorer Detected!", MsgBoxStyle.Critical, "WTF?!")
            Me.Close()
        End If

        If Not TargetProcess3.Length = 0 Then
            TargetProcess3(0).Kill()
            MsgBox("Reflector Detected!", MsgBoxStyle.Critical, "WTF?!")
            Me.Close()
        End If

        If Not TargetProcess4.Length = 0 Then
            TargetProcess4(0).Kill()
            MsgBox("HxD Detected!", MsgBoxStyle.Critical, "WTF?!")
            Me.Close()
        End If

        If Not TargetProcess5.Length = 0 Then
            TargetProcess5(0).Kill()
            MsgBox("Ollydb Detected!", MsgBoxStyle.Critical, "WTF?!")
            Me.Close()
        End If
    End Sub
    Private Function AuthorizeComputer(ByVal pPassPhrase As String, ByVal pUsername As String, ByVal pRegCode As String) As Boolean
        Try
            Dim strMotherboardID As String = Encrypt(pPassPhrase, cHardware.GetMotherBoardID.Trim)
            Dim oReg As Microsoft.Win32.RegistryKey
            oReg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", True)
            oReg = oReg.CreateSubKey(kstrRegSubKeyName)
            oReg.SetValue("USERID", Encrypt(pPassPhrase, pUsername))
            oReg.SetValue("LOCALPATH", Encrypt(pPassPhrase, pRegCode))
            oReg.SetValue("Enabled", "")
            'مخزن هاتين القيمتين، فإن برنامج فحص لكود التحقق في كل شوط
            'فقط لتكون آمنة. : Dev-PoinT Love :)
            oReg.SetValue("CompID", strMotherboardID)
            oReg.Close()
            MessageBox.Show("The license of your application is now saved.", "Licensing demo", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End
            Return True
        Catch ex As Exception
            MessageBox.Show("Impossible to save license info", "Licensing demo", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function
    Private Function DisplayApplicationStatus(ByVal pDaysUsed As Integer, ByVal pTotalDays As Integer) As String
        'تخليه يظهر هذي الرسالة إذا كانت النسخة التجريبية أقل من الصفر
        'معرفة ما اذا كان أدلى مقدم البلاغ في خطأ تعيين يوماً من الفترة التجريبية إلى أقل من 0
        If pTotalDays < 0 Then
            Return "An error has occurred! The author has alloted you a trial period less than zero days, which is impossible. Please contact the author and tell him/her of this error."
        End If

        'تحقق مما إذا انتهت مدة صلاحية النسخة التجريبية
        If pDaysUsed >= pTotalDays Then
            Return "Your trial has expired!"
            MessageBox.Show("Your trial has expired!", "Error ", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End
        End If

        'معرفة كم بقي يوم من النسخة التجريبية طبعاً سوف تظهر للمستخدم حتى يعلم
        Return "You have " + (pTotalDays - pDaysUsed).ToString + " days remaining in your free trial period."
    End Function

    Private Sub CreateRegKeys(ByVal pPassPhrase As String)
        Try
            'تصحيح مولد تسجيل المفاتيح وهي PassPhrase
            Dim Current As CurrentDate
            Current.Day = DateTime.Now.Day.ToString
            Current.Month = DateTime.Now.Month.ToString
            Current.Year = DateTime.Now.Year.ToString
            'If EncryptKeys = True Then
            Current.Day = Encrypt(pPassPhrase, Current.Day)
            Current.Month = Encrypt(pPassPhrase, Current.Month)
            Current.Year = Encrypt(pPassPhrase, Current.Year)
            'End If
            Dim oReg As Microsoft.Win32.RegistryKey
            oReg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", True)
            oReg = oReg.CreateSubKey(kstrRegSubKeyName)
            oReg.SetValue("UserSettings", Current.Day)
            oReg.SetValue("operatingsystem", Current.Month)
            oReg.SetValue("GUID", Current.Year)
            oReg.Close()
        Catch
        End Try
    End Sub
    Private Function DiffDate(ByVal OrigDay As String, ByVal OrigMonth As String, ByVal OrigYear As String) As Integer
        Try
            Dim D1 As Date = New Date(Convert.ToInt32(OrigYear), Convert.ToInt32(OrigMonth), Convert.ToInt32(OrigDay))
            Return Convert.ToInt32(DateDiff(DateInterval.Day, D1, DateTime.Now))
        Catch
            Return 0
        End Try
    End Function
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
    Private Sub btnTestKey_Click(sender As System.Object, e As System.EventArgs) Handles btnTestKey.Click
        Try
            lblVKStatus.Text = ""
            Dim strRegCode As String = Me.txtKeyToValidate.Text.ToUpper
            Dim strPassPhrase As String = txtPassPhrase.Text
            Dim strUserName As String = Encrypt(strPassPhrase, txtVKClientName.Text.ToUpper)
            strUserName = strUserName.Remove(16, (strUserName.Length - 16))
            strRegCode = strRegCode.Replace("-", "")
            If strUserName = strRegCode Then
                AuthorizeComputer(strPassPhrase, txtVKClientName.Text.ToUpper, strRegCode)
            Else
                lblVKStatus.ForeColor = Color.Red
                lblVKStatus.Text = "The name and the key entered appears to be incorrect!"
            End If
        Catch ex As Exception
            lblVKStatus.ForeColor = Color.Blue
            lblVKStatus.Text = "Please fill out the form free!"
        End Try
    End Sub
    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        End
    End Sub
End Class
