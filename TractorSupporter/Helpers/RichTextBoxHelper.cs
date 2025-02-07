﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TractorSupporter.Helpers
{
    public static class RichTextBoxHelper
    {
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.RegisterAttached(
                "Document",
                typeof(FlowDocument),
                typeof(RichTextBoxHelper),
                new PropertyMetadata(null, OnDocumentChanged));

        public static FlowDocument GetDocument(DependencyObject obj)
        {
            return (FlowDocument)obj.GetValue(DocumentProperty);
        }

        public static void SetDocument(DependencyObject obj, FlowDocument value)
        {
            obj.SetValue(DocumentProperty, value);
        }

        private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox richTextBox)
            {
                var newDocument = e.NewValue as FlowDocument;
                if (newDocument != null)
                {
                    var previousParent = LogicalTreeHelper.GetParent(newDocument) as RichTextBox;
                    if (previousParent != null)
                    {
                        previousParent.Document = new FlowDocument();
                    }
                }

                richTextBox.Document = newDocument ?? new FlowDocument();
            }
        }
    }
}