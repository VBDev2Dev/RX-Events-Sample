Imports System.Reactive.Linq

Public Class frmRX
    ''' <summary>
    ''' Drag form by using client area of form
    ''' </summary>
    Public Sub New()
        MyBase.New

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'TurnOnDiagnostics = True
    End Sub

    Private Sub frmRX_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        Dim imgForm As New frmImage()
        Me.Owner = imgForm
        imgForm.Show()



        Dim mDown = From eDown In formMouseDown.Events
                    Select MouseEventArgsInfo.FromMouseEventArgs(eDown.EventArgs, eDown.Sender)

        Dim mUp = From eUp In formMouseDown.Events
                  Select MouseEventArgsInfo.FromMouseEventArgs(eUp.EventArgs, eUp.Sender)

        Dim mMove = From eMv In formMouseMove.Events
                    Select MouseEventArgsInfo.FromMouseEventArgs(eMv.EventArgs, eMv.Sender)


        Dim oStop = From u In mUp ' build observable that will stop drag drop like behaviour 
                    Select u.EventArgs.Button



        Dim mover = (From st In mDown
                     From mv In mMove.Where(Function(m) m.EventArgs.Button.HasFlag(st.EventArgs.Button)).
                         StartWith(st).
                         TakeUntil(oStop.Where(Function(u) u.HasFlag(st.EventArgs.Button)))
                     Select New With {
                         .Move = mv,
                         .Start = st,
                         .Distance = Math.Abs(Math.Sqrt(Math.Pow((mv.Location.X - st.Location.X), 2) + Math.Pow((mv.Location.Y - st.Location.Y), 2)))
                         }).'Where(Function(m) m.Distance >= 10).
                         Repeat









        subscriptions.Add("Move Coordinates", mMove.Subscribe(Sub(msE)
                                                                  lblCoordinates.Text = $"Screen:{msE.ScreenLocation} Location:{msE.Location}"

                                                              End Sub))


        subscriptions.Add("Mouse Down Coordinates", mDown.Subscribe(Sub(msE)
                                                                        Debug.WriteLine($"Mouse Down on Screen:{msE.ScreenLocation} Location:{msE.Location}")

                                                                    End Sub))


        subscriptions.Add("MouseUp Coordinates", mUp.Subscribe(Sub(msE)
                                                                   Debug.WriteLine($"Mouse Up on Screen:{msE.ScreenLocation} Location:{msE.Location}")

                                                               End Sub))

        subscriptions.Add("Mover", mover.Subscribe(Sub(msE)

                                                       Dim diff = msE.Start.ControlLocation - msE.Start.ScreenLocation
                                                       Dim MoveTo = PointToScreen(msE.Move.Location) + diff
                                                       Me.Location = MoveTo
                                                       'Debug.WriteLine($"Distance:{msE.Distance}")
                                                   End Sub))
        Activate()




    End Sub



End Class
