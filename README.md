# Touch Gesture Recognition System (VB.NET)

A touch gesture recognition system for VB.NET programs to enable touchscreen support.

## Installation

1. Include the `Touch Gesture Recognition System.dll` as a reference in your project.
2. (Extra) Change dimensions of the recognition system.


## Example (VB.NET)
```vbnet
Imports Touch_Gesture_Recognition_System 'Import the Touch Gesture Recognition System namespace

Public Class TestForm

    Private mouseFilter As Touch_function 'Create an instance of the Touch_function class

    Private Sub TestForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        mouseFilter = New Touch_function() 'Initialize the Touch_function class
        Application.AddMessageFilter(mouseFilter) 'Add the Touch_function as a message filter to the application
        AddHandler mouseFilter.Recognition, AddressOf TouchHandler 'Add an event handler for gesture recognition
    End Sub

    Private Sub TouchHandler(ByVal sender As Object, ByVal e As GestureArgs) 'Event handler for gesture recognition
        If e.SwipeDirection = "ESU" Then 'Check if the swipe direction is "ESU" (Swipe Up)
            MsgBox("Swipe Up") 'Do an action indicating a swipe up gesture
        End If
    End Sub
End Class
