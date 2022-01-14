Option Strict On
'Coded By : U A C O D E R
Imports System.Management
'Coded By : U A C O D E R
Public Class cHardware

    Public Shared Function GetProcessorId() As String
        Dim strProcessorId As String = String.Empty
        Dim query As New SelectQuery("Win32_processor")
        Dim search As New ManagementObjectSearcher(query)

        For Each info As ManagementObject In search.Get()
            strProcessorId = info("processorId").ToString()
        Next

        Return strProcessorId
    End Function

    Public Shared Function GetMotherBoardID() As String
        Dim strMotherBoardID As String = String.Empty
        Dim query As New SelectQuery("Win32_BaseBoard")
        Dim search As New ManagementObjectSearcher(query)

        For Each info As ManagementObject In search.Get()
            strMotherBoardID = info("SerialNumber").ToString()
        Next

        Return strMotherBoardID
    End Function

End Class
