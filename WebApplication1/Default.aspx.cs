using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using WebApplication1;
namespace WebApplication1
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var contect = new CertificatesEntities();
            contect.Grade10_Student_Details.Add(new Grade10_Student_Details { 
            
            
            });
        }
    }
}