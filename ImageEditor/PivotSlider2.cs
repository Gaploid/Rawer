using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls.Primitives;
using Telerik.Windows.Controls;

namespace ImageEditor
{
   
   
        /// <summary>
        /// The default UI for several <see cref="T:Telerik.Windows.Controls.RadImageEditor" /> tools.
        /// </summary>
        public class PivotSlider2 : RadControl
        {
            /// <summary>
            /// Identifies the <see cref="P:Telerik.Windows.Controls.PivotSlider.MaxValue" /> dependency properties.
            /// </summary>
            public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(PivotSlider2), new PropertyMetadata(0.0));
            /// <summary>
            /// Identifies the <see cref="P:Telerik.Windows.Controls.PivotSlider.Value" /> dependency properties.
            /// </summary>
            public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(PivotSlider2), new PropertyMetadata(0.0, new PropertyChangedCallback(PivotSlider2.OnValueChanged)));
            private GaugeIndicator thumb;
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public double Value
            {
                get
                {
                    return (double)base.GetValue(PivotSlider2.ValueProperty);
                }
                set
                {
                    base.SetValue(PivotSlider2.ValueProperty, value);
                }
            }

            double minMaxDiff;
            int i;

            /// <summary>
            /// Gets or sets the max value. The min value is the negative max value.
            /// </summary>
            public double MaxValue
            {
                get
                {
                    return (double)base.GetValue(PivotSlider2.MaxValueProperty);
                }
                set
                {
                    base.SetValue(PivotSlider2.MaxValueProperty, value);
                }
            }
            /// <summary>
            /// Initializes a new instance of the PivotSlider class.
            /// </summary>
            public PivotSlider2()
            {
                base.DefaultStyleKey = typeof(PivotSlider2);
            }
            /// <summary>
            /// Called before the System.Windows.UIElement.Tap event occurs.For information
            /// on using gestures on Windows Phone, see How to handle manipulation events
            /// for Windows Phone.
            /// </summary>
            /// <param name=
            /// "e">Event data for the event.</param>
            //protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
            //{
            //    base.OnManipulationDelta(e);
            //    if (e.Handled) {
            //        return;                
            //    }

            //    double normalizesPos = e.DeltaManipulation.Translation.X / base.ActualWidth;
            //    this.Value = (this.thumb.Value = this.thumb.Owner.MinValue + (this.thumb.Owner.MaxValue - this.thumb.Owner.MinValue) * normalizesPos);
            

            //}
            
            //protected override void OnTap(GestureEventArgs e)
            //{
            //    base.OnTap(e);
            //    if (e.get_Handled())
            //    {
            //        return;
            //    }
            //    double normalizesPos = e.GetPosition(this).get_X() / base.get_ActualWidth();
            //    this.Value = (this.thumb.Value = this.thumb.Owner.MinValue + (this.thumb.Owner.MaxValue - this.thumb.Owner.MinValue) * normalizesPos);
            //}
            /// <summary>
            /// When overridden in a derived class, is invoked whenever application code
            /// or internal processes (such as a rebuilding layout pass) call System.Windows.Controls.Control.ApplyTemplate().
            /// In simplest terms, this means the method is called just before a UI element
            /// displays in an application. For more information, see Remarks.
            /// </summary>
            public override void OnApplyTemplate()
            {
                base.OnApplyTemplate();
                if (this.thumb != null)
                {
                    this.thumb.ManipulationDelta -= OnThumbManipulationDelta;
                    this.thumb.ManipulationCompleted -= OnThumbManipulationCompleted;
                    this.thumb.ManipulationStarted -= thumb_ManipulationStarted;
                //    this.thumb.remove_ManipulationCompleted(new EventHandler<ManipulationCompletedEventArgs>(this.OnThumbManipulationCompleted));
                }
                this.thumb = base.GetTemplatePart<GaugeIndicator>("PART_Thumb", true);
                this.thumb.ManipulationDelta+= OnThumbManipulationDelta;
                this.thumb.ManipulationCompleted+= OnThumbManipulationCompleted;
                this.thumb.ManipulationStarted += thumb_ManipulationStarted;
            }

            void thumb_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
            {
                
                minMaxDiff = this.thumb.Owner.MaxValue - this.thumb.Owner.MinValue;
                //throw new NotImplementedException();
            }
            /// <summary>
            /// A virtual callback that is called when the Value property changes.
            /// </summary>
            /// <param name="newValue">The new property value.</param>
            /// <param name="oldValue">The old property value.</param>
            protected virtual void OnValueChanged(double newValue, double oldValue)
            {
                if (!base.IsLoaded || !base.IsTemplateApplied)
                {
                    return;
                }
                this.thumb.Value = newValue;
            }
            private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                (d as PivotSlider2).OnValueChanged((double)e.NewValue, (double)e.OldValue);
            }
            private void OnThumbManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
            {
                this.Value = this.thumb.Value;
            }
            private void OnThumbManipulationDelta(object sender, ManipulationDeltaEventArgs e)
            {
               
                double move = e.DeltaManipulation.Translation.X / base.ActualWidth * minMaxDiff;
                //double move2 = e.CumulativeManipulation.Translation.X / base.ActualWidth * minMaxDiff;
                this.thumb.Value += move;

                
                this.Value += move;

            }
        }
    }


