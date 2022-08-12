Imports System.Reactive
''' <summary>
''' Observable source from an event.
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class ObservableEvent(Of T)
    Property EventName As String
    Property Control As Control
    Property Events As IObservable(Of EventPattern(Of T))


End Class