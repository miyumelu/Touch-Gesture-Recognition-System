Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public Delegate Sub Gesture(ByVal sender As Object, ByVal e As GestureArgs)

Public Class GestureArgs
    Inherits EventArgs

    Public Property SwipeDirection As String
    Public Property AxisY As Integer

    Public Sub New(direction As String)
        SwipeDirection = direction
    End Sub

    Public Sub New(pointY As Integer)
        AxisY = pointY
    End Sub

End Class

Public Class Touch_function
    Implements IMessageFilter

    Private ReadOnly screenHeight As Integer = Screen.PrimaryScreen.Bounds.Height
    Private ReadOnly screenWidth As Integer = Screen.PrimaryScreen.Bounds.Width
    Private ReadOnly PercentHeight As Integer
    Private ReadOnly thirtyPercentWidth As Integer

    Private Const WM_LBUTTONDOWN As Integer = &H201
    Private Const WM_LBUTTONUP As Integer = &H202
    Private Const WM_LBUTTONDBLCLK As Integer = &H203

    Private Const LONG_PRESS_MS As Integer = 600
    Private WithEvents _longPressTimer As New Timer()
    Private _longPressTriggered As Boolean = False

    Private WithEvents _tapTimer As New Timer()
    Private _pendingTap As Boolean = False

    Private startPosX As Integer
    Private startPosY As Integer

    Public Event Recognition As Gesture
    Public Event TouchPointY As Gesture

    Public Sub New()
        PercentHeight = CInt(screenHeight * 0.2)
        thirtyPercentWidth = CInt(screenWidth * 0.3)

        _longPressTimer.Interval = LONG_PRESS_MS
        _longPressTimer.Stop()

        _tapTimer.Interval = SystemInformation.DoubleClickTime
        _tapTimer.Stop()
    End Sub

    Private Sub LongPressTimer_Tick(sender As Object, e As EventArgs) Handles _longPressTimer.Tick
        _longPressTimer.Stop()
        _longPressTriggered = True
        RaiseEvent Recognition(Me, New GestureArgs("ETX"))  ' Event Touch Large (Long Press)
    End Sub

    Private Sub TapTimer_Tick(sender As Object, e As EventArgs) Handles _tapTimer.Tick
        _tapTimer.Stop()
        If _pendingTap Then
            _pendingTap = False
            RaiseEvent Recognition(Me, New GestureArgs("ET"))  ' Event Touch (Tap)
        End If
    End Sub

    Public Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage

        Select Case m.Msg

            Case WM_LBUTTONDOWN
                Dim mouse As Point = Cursor.Position
                startPosX = mouse.X
                startPosY = mouse.Y

                _longPressTriggered = False
                _longPressTimer.Start()

            Case WM_LBUTTONUP
                _longPressTimer.Stop()

                If _longPressTriggered Then
                    _longPressTriggered = False
                    Return False
                End If

                Dim mouse As Point = Cursor.Position
                Dim diffX As Integer = mouse.X - startPosX
                Dim diffY As Integer = mouse.Y - startPosY

                RaiseEvent TouchPointY(Me, New GestureArgs(startPosY))

                If diffY > PercentHeight * 2 Then
                    RaiseEvent Recognition(Me, New GestureArgs("ESDX"))     ' Event Swipe Down Large

                ElseIf diffY < -(PercentHeight * 2) Then
                    RaiseEvent Recognition(Me, New GestureArgs("ESUX"))     ' Event Swipe Up Large

                ElseIf diffY > PercentHeight Then
                    RaiseEvent Recognition(Me, New GestureArgs("ESD"))      ' Event Swipe Down

                ElseIf diffY < -PercentHeight Then
                    RaiseEvent Recognition(Me, New GestureArgs("ESU"))      ' Event Swipe Up

                ElseIf diffX > thirtyPercentWidth Then
                    RaiseEvent Recognition(Me, New GestureArgs("ESR"))      ' Event Swipe Right

                ElseIf diffX < -thirtyPercentWidth Then
                    RaiseEvent Recognition(Me, New GestureArgs("ESL"))      ' Event Swipe Left

                Else
                    _pendingTap = True
                    _tapTimer.Stop()
                    _tapTimer.Start()

                End If

            Case WM_LBUTTONDBLCLK
                _longPressTimer.Stop()
                _longPressTriggered = False
                _tapTimer.Stop()
                _pendingTap = False

                RaiseEvent Recognition(Me, New GestureArgs("ET2"))          ' Event Touch Twice

        End Select

        Return False

    End Function

    Private Sub Dictionary_Override()

    End Sub

End Class