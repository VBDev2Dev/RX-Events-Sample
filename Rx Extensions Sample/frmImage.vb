Imports System.Drawing.Imaging
Imports System.Reactive.Disposables
Imports System.Reactive.Linq

Public Class frmImage
    ''' <summary>
    ''' Left mouse drw new shape
    ''' Right Mouse clear image on mouse up
    ''' </summary>

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'TurnOnDiagnostics = True
    End Sub

    Dim rand As New Random
    Private Sub frmImage_Shown(sender As Object, e As EventArgs) Handles Me.Shown








        Dim mDown = From eD In RXEventBuilder(Of MouseEventArgs).GetObservableEvent(pbImage, NameOf(pbImage.MouseDown)).Events
                    Select MouseEventArgsInfo.FromMouseEventArgs(eD.EventArgs, eD.Sender)


        Dim mUp = From eU In RXEventBuilder(Of MouseEventArgs).GetObservableEvent(pbImage, NameOf(pbImage.MouseUp)).Events
                  Select MouseEventArgsInfo.FromMouseEventArgs(eU.EventArgs, eU.Sender)

        Dim mMove = From eMv In RXEventBuilder(Of MouseEventArgs).GetObservableEvent(pbImage, NameOf(pbImage.MouseMove)).Events
                    Select MouseEventArgsInfo.FromMouseEventArgs(eMv.EventArgs, eMv.Sender)

        'Mouse moving while mouse down

        Dim mover = (From st In mDown
                     From mv In mMove.
                         StartWith(st).
                         TakeUntil(mUp)
                     Select New With {
                         .Done = False,
                         .Finish = mv,
                         .StartEventArgs = st.EventArgs,
                         .FinishEventArgs = mv.EventArgs,
                         .Start = st,
                         .Limits = New With {
                                       .minX = Math.Min(st.Location.X, mv.Location.X),
                                       .minY = Math.Min(st.Location.Y, mv.Location.Y),
                                       .maxX = Math.Max(st.Location.X, mv.Location.X),
                                       .maxY = Math.Max(st.Location.Y, mv.Location.Y)}
                         }).Repeat

        Dim ImageDataGeneratorFinal = mDown.
                         Zip(mUp, Function(d, u) New With {
                                                .Done = True,
                                                .Finish = u,
                                                .StartEventArgs = d.EventArgs,
                                                .FinishEventArgs = u.EventArgs,
                                                .Start = d,
                                                .Limits = New With {
                                                              .minX = Math.Min(d.Location.X, u.Location.X),
                                                              .minY = Math.Min(d.Location.Y, u.Location.Y),
                                                              .maxX = Math.Max(d.Location.X, u.Location.X),
                                                              .maxY = Math.Max(d.Location.Y, u.Location.Y)}
                                 })


        Dim GetFreshBitmap = Function()
                                 Return New Bitmap(pbImage.Width, pbImage.Height, PixelFormat.Format32bppArgb)
                             End Function

        Dim ImageMaker = Observable.Using(Function() New SerialDisposable() With {.Disposable = GetFreshBitmap()},
                                          Function(bmp As SerialDisposable)



                                              Return From msE In ImageDataGeneratorFinal.
                                                         Merge(mover).
                                                         Select(Function(msEs)

                                                                    Dim tmp As Bitmap
                                                                    If msEs.FinishEventArgs.Button = MouseButtons.Right Then
                                                                        tmp = GetFreshBitmap()
                                                                    Else
                                                                        tmp = DirectCast(bmp.Disposable, Bitmap).Clone
                                                                    End If




                                                                    If (msEs.FinishEventArgs.Button = MouseButtons.Right AndAlso Not msEs.Done) OrElse msEs.FinishEventArgs.Button <> MouseButtons.Right Then ' don't do this if right or done
                                                                        Using g As Graphics = Graphics.FromImage(tmp)

                                                                            Dim UsablePens = {Pens.Cyan, Pens.RoyalBlue, Pens.CornflowerBlue, Pens.Red}
                                                                            Dim pen = UsablePens(rand.Next(0, UsablePens.Length))

                                                                            Dim locations = {msEs.Start.Location, msEs.Finish.Location}
                                                                            Dim StartRect = Rectangle.FromLTRB(locations(0).X - 20, locations(0).Y - 20, locations(0).X + 20, locations(0).Y + 20)
                                                                            Dim EndRect = Rectangle.FromLTRB(locations(1).X - 20, locations(1).Y - 20, locations(1).X + 20, locations(1).Y + 20)
                                                                            Dim rectArea = Rectangle.FromLTRB(msEs.Limits.minX, msEs.Limits.minY, msEs.Limits.maxX, msEs.Limits.maxY)

                                                                            g.DrawRectangle(Pens.Blue, StartRect)
                                                                            g.DrawRectangle(Pens.Green, EndRect)
                                                                            g.DrawLine(pen, New Point(msEs.Limits.minX, msEs.Limits.minY), New Point(msEs.Limits.maxX, msEs.Limits.maxY))
                                                                            g.DrawLine(pen, New Point(msEs.Limits.maxX, msEs.Limits.minY), New Point(msEs.Limits.minX, msEs.Limits.maxY))
                                                                            g.DrawRectangle(pen, rectArea)
                                                                            g.DrawEllipse(pen, rectArea)
                                                                        End Using
                                                                    End If
                                                                    If msEs.Done Then
                                                                        If msEs.FinishEventArgs.Button <> MouseButtons.Right Then
                                                                            bmp.Disposable = tmp
                                                                        Else
                                                                            bmp.Disposable = GetFreshBitmap()
                                                                        End If
                                                                    End If


                                                                    Return New With {.Data = msEs, .Image = tmp}
                                                                End Function)
                                          End Function)




        subscriptions.Add("GenImage", ImageMaker.Subscribe(Sub(IGen)

                                                               RefreshImage(IGen.Image)

                                                           End Sub))







    End Sub
    Sub RefreshImage(Replacement As Image)
        pbImage.Image = Replacement
    End Sub


    Protected Overrides Sub AddDiagnostics()
        MyBase.AddDiagnostics()

        Dim mDown = From eD In RXEventBuilder(Of MouseEventArgs).GetObservableEvent(pbImage, NameOf(pbImage.MouseDown)).Events
                    Select MouseEventArgsInfo.FromMouseEventArgs(eD.EventArgs, eD.Sender)


        Dim mUp = From eU In RXEventBuilder(Of MouseEventArgs).GetObservableEvent(pbImage, NameOf(pbImage.MouseUp)).Events
                  Select MouseEventArgsInfo.FromMouseEventArgs(eU.EventArgs, eU.Sender)



        subscriptions.Add("DiagImageDown", mDown.Subscribe(Sub(evt)
                                                               Debug.WriteLineIf(TurnOnDiagnostics, "DiagImageDown")
                                                               Debug.WriteLineIf(TurnOnDiagnostics, evt)
                                                           End Sub))

        subscriptions.Add("DiagUmageUp", mUp.Subscribe(Sub(evt)
                                                           Debug.WriteLineIf(TurnOnDiagnostics, "DiagUmageUp")
                                                           Debug.WriteLineIf(TurnOnDiagnostics, evt)
                                                       End Sub))



    End Sub

End Class