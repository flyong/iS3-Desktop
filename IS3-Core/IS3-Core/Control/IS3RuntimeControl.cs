using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace iS3.Core
{

    public enum PageType { LoginPage,ProjectList,MainFrame}
    public class IS3RuntimeControl
    {
        private IS3PageHolder myPageHolder;
        public void SetPageHolder(IS3PageHolder PageHolder)
        {
            this.myPageHolder = PageHolder;
        }
        public void SetPageShow(PageType type)
        {
            if (myPageHolder != null)
            {
                switch (type) {
                    case PageType.LoginPage:
                        if (myLoign != null)
                            myPageHolder.ShowPage(myLoign as UserControl);
                        break;
                    case PageType.ProjectList:
                        if (myProjectList != null)
                            myPageHolder.ShowPage(myProjectList as UserControl);
                        break;
                    case PageType.MainFrame:
                        if (myMainFrame != null)
                            myPageHolder.ShowPage(myMainFrame as UserControl);
                        break;
                    default:break;
                }
            }
            
        }
        public void MainWindowShow()
        {
            if (myPageHolder != null)
            {
                myPageHolder.SetShow();
            }
        }
        public void SetPageTransition(IPageTransition pageTransition)
        {
            if (myPageHolder != null)
            {
                myPageHolder.SetPageTransition(pageTransition);
            }
        }
        private ILogin myLoign;
        public void SetLogin(ILogin Loign)
        {
            this.myLoign = Loign;
            Loign.UserLoginFinished += LoginFinishedListener;
        }
        private IProjectList myProjectList;
        public void SetProjectList(IProjectList ProjectList)
        {
            this.myProjectList = ProjectList;
            myProjectList.ProjectViewTriggle += ProjectViewListener;
        }
        private IMainFrame myMainFrame;
        public void SetMainFrame(IMainFrame MainFrame)
        {
            this.myMainFrame = MainFrame;
        }
        public void LoginFinishedListener(object sender, UserLogin e)
        {
            SetPageShow(PageType.ProjectList);
        }
        public void ProjectViewListener(object sender, Project e)
        {
            myMainFrame.projectID = e.projectID;
            SetPageShow(PageType.MainFrame);
        }
    }
}
