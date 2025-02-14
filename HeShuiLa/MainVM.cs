using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeShuiLa
{
    public class MainVM : ObservableObject
    {
        private string hintText = "Please drink water";
        public string HintText { get => hintText; set => SetProperty(ref hintText, value, nameof(HintText)); }
    }
}
