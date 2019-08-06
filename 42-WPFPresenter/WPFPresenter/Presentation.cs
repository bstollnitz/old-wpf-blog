using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace WPFPresenter
{
    public class Presentation : INotifyPropertyChanged
    {
        private string[] slides = { "01Title.xaml", "02Slide.xaml", "03Slide.xaml" };
        private int currentIndex = 0;

        public string[] Slides
        {
            get { return this.slides; }
        }

        public string CurrentSlide
        {
            get { return this.slides[this.currentIndex]; }
        }

        public int CurrentIndex
        {
            get { return this.currentIndex; }
            set
            {
                if (this.currentIndex != value)
                {
                    this.currentIndex = value;
                    this.OnPropertyChanged("CurrentSlide");
                    this.OnPropertyChanged("CanGoBack");
                    this.OnPropertyChanged("CanGoNext");
                }
            }
        }

        public bool CanGoBack
        {
            get { return this.currentIndex > 0; }
        }

        public bool CanGoNext
        {
            get { return this.currentIndex < this.slides.Length - 1; }
        }

        public void GoBack()
        {
            if (this.CanGoBack)
            {
                this.CurrentIndex--;
            }
        }

        public void GoNext()
        {
            if (this.CanGoNext)
            {
                this.CurrentIndex++;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
