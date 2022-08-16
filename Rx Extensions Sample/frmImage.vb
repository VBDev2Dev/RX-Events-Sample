Imports System.Drawing.Drawing2D
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


        Dim angle As Integer





        Dim mDown = From eD In RXEventBuilder(Of MouseEventArgs).GetObservableEvent(pbImage, NameOf(pbImage.MouseDown)).Events
                    Select MouseEventArgsInfo.FromMouseEventArgs(eD.EventArgs, eD.Sender)


        Dim mUp = From eU In RXEventBuilder(Of MouseEventArgs).GetObservableEvent(pbImage, NameOf(pbImage.MouseUp)).Events
                  Select MouseEventArgsInfo.FromMouseEventArgs(eU.EventArgs, eU.Sender)

        Dim mMove = From eMv In RXEventBuilder(Of MouseEventArgs).GetObservableEvent(pbImage, NameOf(pbImage.MouseMove)).Events
                    Select MouseEventArgsInfo.FromMouseEventArgs(eMv.EventArgs, eMv.Sender)

        Dim oStop = From u In mUp ' build observable that will stop drag drop like behaviour 
                    Select u.EventArgs.Button

        'Mouse moving while mouse down
        Dim mover = (From st In mDown.Take(1)
                     From mv In mMove.
                         Where(Function(m) m.EventArgs.Button.HasFlag(st.EventArgs.Button)).
                         StartWith(st).
                         TakeUntil(oStop.Where(Function(u) u.HasFlag(st.EventArgs.Button)))
                     Select New RXMouseRangeInfo With {
                                 .Done = False,
                                 .Finish = mv,
                                 .StartEventArgs = st.EventArgs,
                                 .FinishEventArgs = mv.EventArgs,
                                 .Start = st
                            }).
                         Repeat

        Dim ImageDataGeneratorFinal = mDown.
                         Zip(mUp, Function(d, u) New RXMouseRangeInfo With {
                                                    .Done = True,
                                                    .Finish = u,
                                                    .StartEventArgs = d.EventArgs,
                                                    .FinishEventArgs = u.EventArgs,
                                                    .Start = d
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

                                                                            Dim UsablePens = {Pens.Purple, Pens.Cyan}
                                                                            Dim pen = UsablePens(rand.Next(0, UsablePens.Length))

                                                                            Dim locations = {msEs.Start.Location, msEs.Finish.Location}
                                                                            Dim StartRect = Rectangle.FromLTRB(locations(0).X - 20, locations(0).Y - 20, locations(0).X + 20, locations(0).Y + 20)
                                                                            Dim EndRect = Rectangle.FromLTRB(locations(1).X - 20, locations(1).Y - 20, locations(1).X + 20, locations(1).Y + 20)
                                                                            Dim rectArea = Rectangle.FromLTRB(msEs.Limits.MinX, msEs.Limits.MinY, msEs.Limits.MaxX, msEs.Limits.MaxY)
                                                                            Dim midRect = Rectangle.FromLTRB(msEs.Midpoint.Value.X - 10, msEs.Midpoint.Value.Y - 10, msEs.Midpoint.Value.X + 10, msEs.Midpoint.Value.Y + 10)
                                                                            Dim offsetBy As New Point(msEs.Limits.MinX - (msEs.Limits.MinX + msEs.Midpoint.Value.X) / 2, msEs.Limits.MinY - (msEs.Limits.MinY + msEs.Midpoint.Value.Y) / 2)
                                                                            Dim offsetLimits = {
                                                                                            New Limit With {
                                                                                                .MinX = msEs.Limits.MinX + offsetBy.X,
                                                                                                .MaxX = msEs.Limits.MaxX + offsetBy.X,
                                                                                                .MinY = msEs.Limits.MinY + offsetBy.Y,
                                                                                                .MaxY = msEs.Limits.MaxY + offsetBy.Y
                                                                                                },
                                                                                            New Limit With {
                                                                                                .MinX = msEs.Limits.MinX - offsetBy.X,
                                                                                                .MaxX = msEs.Limits.MaxX - offsetBy.X,
                                                                                                .MinY = msEs.Limits.MinY - offsetBy.Y,
                                                                                                .MaxY = msEs.Limits.MaxY - offsetBy.Y
                                                                                                }
                                                                                            }
                                                                            Dim rectAreaOffset = offsetLimits.Select(Function(ol) Rectangle.FromLTRB(ol.MinX, ol.MinY, ol.MaxX, ol.MaxY))
                                                                            Dim LabeledPoints = {
                                                                                                New Point(offsetLimits(0).MinX, offsetLimits(0).MinY),
                                                                                                New Point(offsetLimits(0).MaxX, offsetLimits(0).MinY),
                                                                                                New Point(offsetLimits(1).MaxX, offsetLimits(1).MinY),
                                                                                                New Point(offsetLimits(1).MinX, offsetLimits(1).MinY),
                                                                                                New Point(offsetLimits(1).MinX, offsetLimits(1).MaxY),
                                                                                                New Point(offsetLimits(1).MaxX, offsetLimits(1).MaxY),
                                                                                                New Point(offsetLimits(0).MaxX, offsetLimits(0).MaxY),
                                                                                                New Point(offsetLimits(0).MinX, offsetLimits(0).MaxY)
                                                                            }.
                                                         Select(Function(p, i) New With {.Index = i, .Point = p}).
                                                         ToDictionary(Function(p) ChrW(p.Index + 65), Function(p) p.Point)

                                                                            Dim midpointsTemp = (From lp In LabeledPoints
                                                                                                 From lp2 In LabeledPoints
                                                                                                 Where lp.Key <> lp2.Key
                                                                                                 Select New With {Key .Key = New String({lp.Key, lp2.Key}.OrderBy(Function(c) c).ToArray), .Point1 = lp.Value, .Point2 = lp2.Value}).DistinctBy(Function(t) t.Key)


                                                                            Dim lines = midpointsTemp.ToDictionary(Function(mt) mt.Key, Function(mt) New With {.Points = {mt.Point1, mt.Point2}, .MidPoint = GetMidpoint(mt.Point1, mt.Point2)})


                                                                            g.FillPath(Brushes.Yellow, New GraphicsPath({LabeledPoints("A"), LabeledPoints("B"), LabeledPoints("G"), LabeledPoints("H")}, {1, 1, 1, 1}))
                                                                            g.FillPath(Brushes.Cyan, New GraphicsPath({LabeledPoints("H"), LabeledPoints("G"), LabeledPoints("F"), LabeledPoints("E")}, {1, 1, 1, 1}))
                                                                            g.FillPath(Brushes.Violet, New GraphicsPath({LabeledPoints("B"), LabeledPoints("C"), LabeledPoints("F"), LabeledPoints("G")}, {1, 1, 1, 1}))
                                                                            g.FillPath(Brushes.Violet, New GraphicsPath({LabeledPoints("A"), LabeledPoints("D"), LabeledPoints("E"), LabeledPoints("H")}, {1, 1, 1, 1}))
                                                                            g.FillPath(Brushes.Cyan, New GraphicsPath({LabeledPoints("A"), LabeledPoints("B"), LabeledPoints("C"), LabeledPoints("D")}, {1, 1, 1, 1}))



                                                                            Using fnt As New System.Drawing.Font("Arial", 12.0)
                                                                                If Not msEs.StartEventArgs.Button.HasFlag(MouseButtons.Right) Then

                                                                                    'DrawRotatedText(g,
                                                                                    '    msEs.Midpoint.Value,
                                                                                    '    angle,
                                                                                    '    $"Midpoint:{msEs.Midpoint.Value}{vbCrLf}Distance First to Second:{msEs.Distance.Value}{vbCrLf}Distance to Midpoint:{msEs.MidpointDistance.Value}",
                                                                                    '    fnt,
                                                                                    '    Brushes.Black)
                                                                                    'DrawRotatedText(g, locations(0), 70, $"{locations(0)}", fnt, Brushes.Black)
                                                                                    'DrawRotatedText(g, locations(1), 70, $"{locations(1)}", fnt, Brushes.Black)
                                                                                    For Each kv In LabeledPoints
                                                                                        Dim rect = Rectangle.FromLTRB(kv.Value.X - 30, kv.Value.Y - 30, kv.Value.X + 30, kv.Value.Y + 30)
                                                                                        g.DrawEllipse(Pens.Red, rect)
                                                                                        DrawRotatedText(g, kv.Value, 70, kv.Key, fnt, Brushes.Black)
                                                                                    Next

                                                                                End If

                                                                                'While rectArea.Contains(midRect)
                                                                                '    'g.DrawRectangle(pen, midRect)
                                                                                '    g.DrawEllipse(Pens.Green, midRect)
                                                                                '    midRect.Inflate(New Drawing.Size(10, 10))
                                                                                'End While




                                                                                'g.DrawRectangle(Pens.Blue, StartRect)
                                                                                g.DrawEllipse(Pens.Blue, StartRect)
                                                                                'g.DrawRectangle(Pens.Green, EndRect)
                                                                                g.DrawEllipse(Pens.Green, EndRect)



                                                                                'g.DrawLine(pen, New Point(msEs.Limits.MinX, msEs.Limits.MinY), New Point(msEs.Limits.MaxX, msEs.Limits.MaxY))
                                                                                'g.DrawLine(pen, New Point(msEs.Limits.MaxX, msEs.Limits.MinY), New Point(msEs.Limits.MinX, msEs.Limits.MaxY))


                                                                                'GetMidpoint(New Point(offsetLimits(0).MinX, offsetLimits(1).MinY), New Point(offsetLimits(1).MinX, offsetLimits(1).MinY)),
                                                                                '                    GetMidpoint(New Point(offsetLimits(0).MaxX, offsetLimits(0).MaxY), New Point(offsetLimits(1).MaxX, offsetLimits(1).MaxY)))
                                                                                'g.DrawRectangle(pen, rectArea)

                                                                                'Dim br = GetMidpoint(offsetLimits(x).MaxX, offsetLimits(x).MaxY, msEs.Limits.MaxX, msEs.Limits.MaxY))

                                                                                ' g.DrawEllipse(pen, rectArea)


                                                                                For x As Integer = 0 To 1
                                                                                    g.DrawRectangle(Pens.BlueViolet, rectAreaOffset(x))


                                                                                    g.DrawLine(Pens.Red, offsetLimits(x).MinX, offsetLimits(x).MinY, msEs.Limits.MinX, msEs.Limits.MinY)
                                                                                    g.DrawLine(Pens.Red, offsetLimits(x).MaxX, offsetLimits(x).MaxY, msEs.Limits.MaxX, msEs.Limits.MaxY)
                                                                                    g.DrawLine(Pens.Red, offsetLimits(x).MinX, offsetLimits(x).MaxY, msEs.Limits.MinX, msEs.Limits.MaxY)
                                                                                    g.DrawLine(Pens.Red, offsetLimits(x).MaxX, offsetLimits(x).MinY, msEs.Limits.MaxX, msEs.Limits.MinY)
                                                                                    g.DrawLine(Pens.Red, offsetLimits(x).MaxX, offsetLimits(x).MinY, msEs.Limits.MaxX, msEs.Limits.MinY)


                                                                                Next
                                                                                Debug.WriteLine("")

                                                                                Dim linesIndexed = lines.Select(Function(li, i) New With {.Line = li, Key .Index = i})
                                                                                Dim drawn As New List(Of Rectangle)
                                                                                For Each l In linesIndexed
                                                                                    Dim rect = Rectangle.FromLTRB(l.Line.Value.MidPoint.X - 10, l.Line.Value.MidPoint.Y - 10, l.Line.Value.MidPoint.X + 10, l.Line.Value.MidPoint.Y + 10)



                                                                                    g.DrawEllipse(Pens.Green, rect)
                                                                                    rect.Offset(New Point(-50, -50))

                                                                                    While drawn.Any(Function(d) d.IntersectsWith(rect))
                                                                                        rect.Offset(New Point(-50, -50))
                                                                                    End While
                                                                                    drawn.Add(rect)

                                                                                    If Not msEs.StartEventArgs.Button.HasFlag(MouseButtons.Right) Then
                                                                                        DrawRotatedText(g, rect.Location, 45, l.Line.Key, fnt, Brushes.Black)
                                                                                        g.DrawLine(Pens.Black, rect.Location, l.Line.Value.MidPoint)
                                                                                    End If


                                                                                Next

                                                                                'uncomment to see midpoints connecting with vertices                                                                                
                                                                                'For Each mp In linesIndexed.Select(Function(li) li.Line.Value)
                                                                                '    For Each p In mp.Points
                                                                                '        g.DrawLine(Pens.Lime, p, mp.MidPoint)
                                                                                '    Next

                                                                                'Next



                                                                                g.DrawLine(Pens.Red, lines("AE").MidPoint, lines("AF").MidPoint)
                                                                                g.DrawLine(Pens.Red, lines("AC").MidPoint, lines("AF").MidPoint)
                                                                                g.DrawLine(Pens.Red, lines("BF").MidPoint, lines("AF").MidPoint)
                                                                                g.DrawLine(Pens.Red, lines("EG").MidPoint, lines("AF").MidPoint)


                                                                                g.DrawPath(Pens.Black, New GraphicsPath({lines("AE").MidPoint, lines("AC").MidPoint, lines("BF").MidPoint, lines("EG").MidPoint}, {1, 1, 1, 1 Or 128}))


                                                                                Dim rectCenter = Rectangle.FromLTRB(lines("BC").MidPoint.X - 20, lines("BC").MidPoint.Y - 20, lines("BC").MidPoint.X + 20, lines("BC").MidPoint.Y + 20)
                                                                                rectCenter.Offset(New Point(50, -50))

                                                                                DrawRotatedText(g, rectCenter.Location, 45, "Center of cube", fnt, Brushes.Black)
                                                                                g.DrawLine(Pens.Red, rectCenter.Location, lines("AF").MidPoint)



                                                                            End Using
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
                                          End Function).Sample(TimeSpan.FromMilliseconds(100))




        subscriptions.Add("GenImage", ImageMaker.Subscribe(Sub(IGen)

                                                               RefreshImage(IGen.Image)

                                                           End Sub))
        subscriptions.Add("AngleChange", mDown.Subscribe(Sub()
                                                             angle = rand.Next(0, 360)
                                                             Debug.WriteLine($"Angle:{angle}")
                                                         End Sub))






    End Sub

    Sub DrawRotatedText(ByVal g As Graphics, ByLocation As Point, ByVal angle As Single, ByVal text As String, ByVal font As Font, ByVal brush As Brush)
        Rotate(g, ByLocation, angle)
        Dim size As SizeF = g.MeasureString(text, font)
        g.DrawString(text, font, brush, New PointF(ByLocation.X - size.Width / 2.0F, ByLocation.Y - size.Height / 2.0F))
        g.ResetTransform()

    End Sub


    Function GetMidpoint(rect As Rectangle) As Point

        If rect.Size.Width = 0 OrElse rect.Size.Height = 0 Then Return Point.Empty

        Return New Point(rect.Left + rect.Width / 2, rect.Top + rect.Size.Height / 2)

    End Function
    Function GetMidpoint(p1 As Point, p2 As Point) As Point

        Dim x = p1.X + p2.X
        Dim y = p1.Y + p2.Y
        Return New Point(x / 2, y / 2)



    End Function

    Public Sub Rotate(g As Graphics, ByLocation As Point, angle As Single)
        g.TranslateTransform(ByLocation.X, ByLocation.Y)
        g.RotateTransform(45)
        g.TranslateTransform(-ByLocation.X, -ByLocation.Y)

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