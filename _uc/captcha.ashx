<%@ WebHandler Language="VB" Class="captcha" %>

Imports System
Imports System.Web
Imports System.Drawing

'需加上System.Web.SessionState.IRequiresSessionState
Public Class captcha : Implements IHttpHandler, System.Web.SessionState.IRequiresSessionState
    
    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        'context.Response.ContentType = "text/plain"
        'context.Response.Write("Hello World")
        context.Response.ContentType = "image/jpeg"
        context.Response.Clear()
        
        context.Response.BufferOutput = True
        Dim RandNumber As Long = New Random(CType(DateTime.Now.Ticks Mod System.Int32.MaxValue, Integer)).Next(9999)
        Dim RandString As String = RandNumber.ToString("0000")
        
        context.Session("CAPTCHA") = RandString
        
        Dim CaptchaWidth As Integer = 56
        Dim CaptchaHeight = 20
        
        Dim bitmap As Bitmap = New Bitmap(CaptchaWidth, CaptchaHeight)
        Dim graphics As Graphics = graphics.FromImage(bitmap)
        Dim stringFont As Font = New Font("Arial", 14, System.Drawing.FontStyle.Italic)
        graphics.Clear(Color.White)
        graphics.DrawString(RandString, stringFont, Brushes.Black, 0, 0)
        
        Dim Random As Random = New Random(CType(DateTime.Now.Ticks Mod System.Int32.MaxValue, Integer))
        For i = 1 To 200
            Dim RandPixelX As Long = Random.Next(0, CaptchaWidth)
            Dim RandPixelY As Long = Random.Next(0, CaptchaHeight)
            bitmap.SetPixel(RandPixelX, RandPixelY, Color.Black)
        Next
        
        bitmap.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg)
                
    End Sub
 
    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class