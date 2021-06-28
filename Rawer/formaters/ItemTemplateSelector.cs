using Rawer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Rawer.Formaters
{
   public class ItemTemplateSelector:DataTemplateSelector
    {
        public DataTemplate DefaultTemplate
        {
            get;
            set;
        }

        public DataTemplate LocationURLTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)        {
           
           // bool v = ;
            // Get ViewMode from IsolatedStorageSettings...

            switch (String.IsNullOrEmpty(((MyData)item).URL))
            {
                case true:
                    return DefaultTemplate;

                case false:
                    return LocationURLTemplate;
            }

            return base.SelectTemplate(item, container);
        }


    }

    public abstract class DataTemplateSelector : ContentControl
    {
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }
    }
}
