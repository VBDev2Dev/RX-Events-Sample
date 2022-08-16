# RX-Events-Sample

Shows how RX Extensions can provide complex event handling without using global state.  With code like the following can be used to get an Observable that shows mouse moves while mouse is down.

```vb
Dim mDown = From eDown In formMouseDown.Events
                    Select MouseEventArgsInfo.FromMouseEventArgs(eDown.EventArgs, eDown.Sender)

Dim mUp = From eUp In formMouseDown.Events
                  Select MouseEventArgsInfo.FromMouseEventArgs(eUp.EventArgs, eUp.Sender)

Dim mMove = From eMv In formMouseMove.Events
                    Select MouseEventArgsInfo.FromMouseEventArgs(eMv.EventArgs, eMv.Sender)




Dim mover = (From st In mDown
             From mv In mMove.
                         StartWith(st).
                         TakeUntil(formMouseUp.Events)
                     Select New With {.Move = mv, .Start = st}).Repeat
```

So the way mover works is it starts with a MouseDown event.  Then all the mouse move events are observed till the MouseUp event is observed.  So the startswith puts st or MouseDown event info first.  Then TakeUntil means stop taking events when mouseup happens.  Build the select result per obsevable in mover.  Then wrap all that and call repeat which means after takeuntil finishes obseervable, start another next mousedown.

So there is a small problem with the above code.  What happens when you push multiple buttons down?  The way around this is to do this.

```vb


Dim oStop = From u In mUp ' build observable that will stop drag drop like behaviour 
                    Select u.EventArgs.Button

Dim mover = (From st In mDown
            From mv In mMove.
                         Where(Function(m) m.EventArgs.Button.HasFlag(st.EventArgs.Button)).
                         StartWith(st).
                         TakeUntil(oStop.Where(Function(u) u.HasFlag(st.EventArgs.Button)))
                      Select New With {.Move = mv, .Start = st}).Repeat()

```

What this does is make sure the mouse button pushed down is the mouse button being released and mouse move events are captured with that button being pushed down.  You might find that both buttons down creates weird results from the obsevable.  See if you can find out where you would put ```.Take(1)``` to fix that. oStop could be adjusted to return an observable that handles different events or other observables.  For example, could add MouseLeave event to the mix to stop if dragged beyond border of control.  
