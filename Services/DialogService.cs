﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dame.Services
{
    public interface IDialogService
    {
        bool? ShowConfirmation(string title, string message);
        // Other dialog-related methods
    }
    public class DialogService : IDialogService
    {
        public bool? ShowConfirmation(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }

}
