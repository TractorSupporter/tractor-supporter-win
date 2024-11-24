using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TractorSupporter.Services.Interfaces
{
    public interface INavigationService
    {
        void NavigateToHistory();
        void NavigateToSettings();
        void NavigateToMain();
        void NavigateToStarter();
    }
}
