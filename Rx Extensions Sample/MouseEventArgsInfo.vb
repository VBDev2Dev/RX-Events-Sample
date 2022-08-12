''' <summary>
''' Convert MouseEventArgs to more useful info.
''' </summary>
Public Class MouseEventArgsInfo
    Public Property EventArgs As MouseEventArgs
    Public Property Location As Point
    Public Property ScreenLocation As Point
    Public Property ControlLocation As Point

    Public Shared Function FromMouseEventArgs(args As MouseEventArgs, Control As Control) As MouseEventArgsInfo

        Return New MouseEventArgsInfo With {.EventArgs = args,
        .Location = New Point(args.X, args.Y),
        .ScreenLocation = Control.PointToScreen(New Point(args.X, args.Y)),
        .ControlLocation = Control.Location
        }

    End Function

    Public Overrides Function ToString() As String

        Return $"Location:{Location} ScreenLocation:{ScreenLocation} ControlLocation:{ControlLocation}"
    End Function
End Class




