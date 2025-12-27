Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public Delegate Sub Gesture(ByVal sender As Object, ByVal e As GestureArgs)

Public Class GestureArgs
    Inherits EventArgs
    Public Property SwipeDirection As String
    Public Property _axisY As Integer

    Public Sub New(direction As String)
        SwipeDirection = direction
    End Sub

    Public Sub New(pointy As Integer)
        _axisY = pointy
    End Sub

End Class

Public Class Touch_function

    Implements IMessageFilter ' Implementing IMessageFilter to capture mouse events globally

    Dim screenHeight As Integer = Screen.PrimaryScreen.Bounds.Height
    Dim thirtyPercentHeight As Integer = CInt(screenHeight * 0.3) 'Calculating 30% of all available pixels on the display (scaleable touch)
    Dim screenWidth As Integer = Screen.PrimaryScreen.Bounds.Width
    Dim thirtyPercentWidth As Integer = CInt(screenWidth * 0.3) 'Calculating 30% of all available pixels on the display (scaleable touch)

    Public Event Recognition As Gesture
    Public Event TouchPointY As Gesture

    Private Const WM_LBUTTONDOWN As Integer = &H201 ' Leftmousebutton-down message
    Private Const WM_LBUTTONUP As Integer = &H202   ' Leftmousebutton-up message

    Dim startposy As Integer ' Starting Y position of the touch
    Dim endposy As Integer ' End

    Dim startposx As Integer ' Starting X position of the touch
    Dim endposx As Integer ' End

    Private Sub Dictionary_Override()
        ' Code for automatically setting manually configured screen resolutions, according to CDM
        ' Currently, CDM is still in a design phase, so this function is not yet implemented
    End Sub

    Public Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage

        If m.Msg = WM_LBUTTONDOWN Then

            Dim mouse As Point = Cursor.Position

            startposy = mouse.Y
            startposx = mouse.X

        ElseIf m.Msg = WM_LBUTTONUP Then

            Dim mouse As Point = Cursor.Position

            endposy = mouse.Y
            endposx = mouse.X

            Dim diffy As Integer = endposy - startposy
            Dim diffx As Integer = endposx - startposx

            RaiseEvent TouchPointY(Me, New GestureArgs(startposy))

            If diffy > thirtyPercentHeight * 2 Then
                RaiseEvent Recognition(Me, New GestureArgs("ESDX")) ' Event Swipe Down Large

            ElseIf diffy < -thirtyPercentHeight * 2 Then
                RaiseEvent Recognition(Me, New GestureArgs("ESUX")) ' Event Swipe Up Large

            ElseIf diffy > thirtyPercentHeight Then
                RaiseEvent Recognition(Me, New GestureArgs("ESD")) ' Event Swipe Down

            ElseIf diffy < -thirtyPercentHeight Then
                RaiseEvent Recognition(Me, New GestureArgs("ESU")) ' Event Swipe Up

            ElseIf diffx > thirtyPercentWidth Then ' Horizontal Swipe Detection
                RaiseEvent Recognition(Me, New GestureArgs("ESR")) ' Event Swipe Right

            ElseIf diffx < -thirtyPercentWidth Then ' Horizontal Swipe Detection
                RaiseEvent Recognition(Me, New GestureArgs("ESL")) ' Event Swipe Left
            End If

        End If

        Return False ' Return false to allow other message filters to process the message

    End Function
End Class
