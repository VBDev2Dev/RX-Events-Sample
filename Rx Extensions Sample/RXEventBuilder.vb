Imports System.Reactive.Linq



''' <summary>
''' Makes sure observables are generated with correct observe on an generates the ObservableEvent instances.  
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class RXEventBuilder(Of T)
    ''' <summary>
    ''' Makes sure events are observed on ui thread with ObserveOn
    ''' </summary>
    ''' <param name="control"></param>
    ''' <param name="EventName"></param>
    ''' <returns></returns>
    Public Shared Function GetObservableEvent(control As Control, EventName As String) As ObservableEvent(Of T)
        Return New ObservableEvent(Of T) With {
        .Control = control,
        .EventName = EventName,
        .Events = (From args In Observable.FromEventPattern(Of T)(control, EventName)).
                   ObserveOn(WindowsFormsSynchronizationContext.Current)
        }


    End Function


End Class
