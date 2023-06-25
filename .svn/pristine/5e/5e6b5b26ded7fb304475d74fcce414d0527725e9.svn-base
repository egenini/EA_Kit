using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestFul
{
    class EAUtilsLocal
    {
        private static int TAB__CLOSE = 0; //  to indicate that it is not visible at all
        private static int TAB__OPEN = 1; //  to indicate that it is open but not top-most
        private static int TAB__ACTIVE = 2; //  to indicate that a tab is open and active (top-most)

        private Repository repository;

        public EAUtilsLocal(Repository repository)
        {
            this.repository = repository;
        }

        public void activateTab(string tabName)
        {
            this.repository.ActivateTab(tabName);
        }

        public object addTab(string tabName, string userControl)
        {
            return this.repository.AddTab(tabName, userControl);
        }

        public bool isTabOpen(string tabName)
        {
            return this.repository.IsTabOpen(tabName) == TAB__OPEN;
        }
        public bool isTabActive(string tabName)
        {
            return this.repository.IsTabOpen(tabName) == TAB__ACTIVE;
        }
        public bool isTabClose(string tabName)
        {
            return this.repository.IsTabOpen(tabName) == TAB__CLOSE;
        }

    }
}
