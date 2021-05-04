﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sharp_Player
{
    class LoadFiles
    {
        private string title;
        private BitmapImage imageData;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        public BitmapImage ImageData
        {
            get
            {
                return imageData;
            }
            set
            {
                imageData = value;
            }
        }
    }
}
