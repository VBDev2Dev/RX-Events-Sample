Imports System.Reactive.Linq

Public Class RXForm
    ''' <summary>
    ''' Base class to hold common items
    ''' </summary>
    Protected WithEvents subscriptions As New Dictionary(Of String, IDisposable)
    Public Sub New()
        MyBase.New



        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.





        formMouseMove = RXEventBuilder(Of MouseEventArgs).GetObservableEvent(Me, NameOf(MouseMove))
        formMouseDown = RXEventBuilder(Of MouseEventArgs).GetObservableEvent(Me, NameOf(MouseDown))
        formMouseUp = RXEventBuilder(Of MouseEventArgs).GetObservableEvent(Me, NameOf(MouseUp))




    End Sub
    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                If components IsNot Nothing Then components.Dispose()
                For Each item In subscriptions.Select(Function(kv) kv.Value)
                    item.Dispose()
                Next
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    ''' <summary>
    ''' Override to add diagnostic subscribers to debug your IObservable instences.
    ''' </summary>
    Protected Overridable Sub AddDiagnostics()
        Dim mDown = From eDown In formMouseDown.Events
                    Select MouseEventArgsInfo.FromMouseEventArgs(eDown.EventArgs, eDown.Sender)

        Dim mUp = From eUp In formMouseUp.Events
                  Select MouseEventArgsInfo.FromMouseEventArgs(eUp.EventArgs, eUp.Sender)

        Dim mMove = From eMv In formMouseMove.Events
                    Select MouseEventArgsInfo.FromMouseEventArgs(eMv.EventArgs, eMv.Sender)

        Dim mover = (From st In mDown
                     From mv In mMove.
                         StartWith(st).
                         TakeUntil(formMouseUp.Events)
                     Select New With {.Move = mv, .Start = st}).Repeat

        subscriptions.Add("DiagFormmDown", mDown.Subscribe(Sub(evt)
                                                               Debug.WriteLineIf(TurnOnDiagnostics, "DiagFormmDown")
                                                               Debug.WriteLineIf(TurnOnDiagnostics, evt)
                                                           End Sub))

        subscriptions.Add("DiagFormmUp", mUp.Subscribe(Sub(evt)
                                                           Debug.WriteLineIf(TurnOnDiagnostics, "DiagFormmUp")
                                                           Debug.WriteLineIf(TurnOnDiagnostics, evt)
                                                       End Sub))


        subscriptions.Add("DiagFormmMove", mover.Subscribe(Sub(evt)
                                                               Debug.WriteLineIf(TurnOnDiagnostics, "DiagFormmMove")
                                                               Debug.WriteLineIf(TurnOnDiagnostics, evt)
                                                           End Sub))
    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)
        MyBase.OnShown(e)
        AddDiagnostics()
    End Sub
    ''' <summary>
    ''' Set to true for diagnostics to show output.
    ''' </summary>
    ''' <returns></returns>
    Protected Property TurnOnDiagnostics As Boolean = False
    Protected ReadOnly formMouseMove As ObservableEvent(Of MouseEventArgs)

    Protected ReadOnly formMouseDown As ObservableEvent(Of MouseEventArgs)

    Protected ReadOnly formMouseUp As ObservableEvent(Of MouseEventArgs)

End Class
