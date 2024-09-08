using System;
using Microsoft.Maui.Handlers;
using UIKit;

namespace PatientRecordsSecurity.Platforms.MacCatalyst
{
	public class EntryCustomization
	{
        public static void Customize()
        {
            EntryHandler.Mapper.AppendToMapping(nameof(Customize), (handler, view) =>
            {
                if (handler.PlatformView is UITextField nativeEntry)
                {
                    // Set border style
                    nativeEntry.BorderStyle = UITextBorderStyle.RoundedRect;

                    // Optional: Customize the frame and appearance
                    nativeEntry.Layer.CornerRadius = 8;
                    nativeEntry.Layer.BorderWidth = 1;
                    nativeEntry.Layer.BorderColor = UIColor.Gray.CGColor;
                }
            });
            // Corrected PickerHandler
            PickerHandler.Mapper.AppendToMapping(nameof(Customize), (handler, view) =>
            {
                if (handler.PlatformView is UIView nativePickerView)
                {
                    // Optional: Customize the frame and appearance of the Picker container
                    nativePickerView.Layer.CornerRadius = 8;
                    nativePickerView.Layer.BorderWidth = 1;
                    nativePickerView.Layer.BorderColor = UIColor.Gray.CGColor;
                }
            });

            // Corrected EditorHandler
            EditorHandler.Mapper.AppendToMapping(nameof(Customize), (handler, view) =>
            {
                if (handler.PlatformView is UITextView nativeEditor)
                {
                    // Optional: Customize the frame and appearance
                    nativeEditor.Layer.CornerRadius = 8;
                    nativeEditor.Layer.BorderWidth = 1;
                    nativeEditor.Layer.BorderColor = UIColor.Gray.CGColor;
                }
            });

            // Corrected DatePickerHandler
            DatePickerHandler.Mapper.AppendToMapping(nameof(Customize), (handler, view) =>
            {
                if (handler.PlatformView is UIDatePicker nativeDatePicker)
                {
                    // Set border style for DatePicker (which uses UITextField)
                    //nativeDatePicker.BorderStyle = UITextBorderStyle.RoundedRect;

                    // Optional: Customize the frame and appearance
                    nativeDatePicker.Layer.CornerRadius = 8;
                    nativeDatePicker.Layer.BorderWidth = 1;
                    nativeDatePicker.Layer.BorderColor = UIColor.Gray.CGColor;
                }
            });

        }
    }
}

