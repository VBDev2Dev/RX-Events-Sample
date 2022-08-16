NotInheritable Class RXMouseRangeInfo
    Property Done As Boolean
    Property Finish As MouseEventArgsInfo
    Property StartEventArgs As MouseEventArgs
    Property FinishEventArgs As MouseEventArgs
    Property Start As MouseEventArgsInfo
    ReadOnly Property Limits As Limit
        Get
            Return New Limit With {
            .MinX = Math.Min(Start.Location.X, Finish.Location.X),
            .MinY = Math.Min(Start.Location.Y, Finish.Location.Y),
            .MaxX = Math.Max(Start.Location.X, Finish.Location.X),
            .MaxY = Math.Max(Start.Location.Y, Finish.Location.Y)
            }
        End Get
    End Property
    ReadOnly Property Midpoint As Lazy(Of Point)
        Get
            Return New Lazy(Of Point)(Function() New Point((Start.Location.X + Finish.Location.X) / 2, (Start.Location.Y + Finish.Location.Y) / 2))
        End Get
    End Property
    ReadOnly Property Distance As Lazy(Of Double)
        Get
            Return New Lazy(Of Double)(Function() Math.Abs(Math.Sqrt(Math.Pow(Finish.Location.X - Start.Location.X, 2) + Math.Pow(Finish.Location.Y - Start.Location.Y, 2))))
        End Get
    End Property

    ReadOnly Property MidpointDistance As Lazy(Of Double)
        Get
            Return New Lazy(Of Double)(Function() Math.Abs(Math.Sqrt(Math.Pow(Midpoint.Value.X - Start.Location.X, 2) + Math.Pow(Midpoint.Value.Y - Start.Location.Y, 2))))
        End Get
    End Property



End Class
