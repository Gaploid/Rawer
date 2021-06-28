using Rawer.Resources;
//using Rawer.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Reminders;

namespace Rawer
{
    class Reminder
    {
        RadRateApplicationReminder radRateApplicationReminder;
        MessageBoxInfoModel msgBox;
        public Reminder()
        {

            msgBox = new MessageBoxInfoModel()
            {
                Buttons = MessageBoxButtons.YesNo,
                Content = AppResources.ReminderContent,
                SkipFurtherRemindersMessage = AppResources.ReminderSkipFurther,
                Title = AppResources.ReminderSkipFurtherTitle
            };

            radRateApplicationReminder = new RadRateApplicationReminder();
            radRateApplicationReminder.RecurrencePerUsageCount = 3;
            radRateApplicationReminder.AllowUsersToSkipFurtherReminders = true;
            radRateApplicationReminder.MessageBoxInfo = msgBox;
            
        }

        public void ShowReminder(){

            radRateApplicationReminder.Notify();
        
        }
    }
}
