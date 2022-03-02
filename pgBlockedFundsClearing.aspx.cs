//@tndhliwayo 2020

using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Net.Mail;

public partial class pgBlockedFundsClearing : System.Web.UI.Page
{

    static String request = null;
    public static string FCCstatus;
    public static string requestID;
    protected void Page_Load(object sender, EventArgs e)
    {
        string UnitID = string.Empty;
        string ClientID = string.Empty;
        string AccessLevel = string.Empty;
        string UserID = string.Empty;

        if (!IsPostBack)
        {
            // get username from the login field.
            try
            {
                string username = (string)Session["MyLoginSession"];
                ArrayList aLogins = (ArrayList)Session[username + "OMG"];
                lblUsername.Text = (string)aLogins[0];

                AccessLevel = (string)aLogins[1];
                UnitID = (string)aLogins[2];
                ClientID = (string)aLogins[4];
                UserID = ((string)aLogins[0]).Substring(0, ((string)aLogins[0]).Length - 10);

                

            }
            catch (Exception)
            {

                Response.Redirect("Login.aspx");
            }

            //GetBlockedFunds();
        }

    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string FCCResponse = null;
        string response_message = "null";
        string status;

        // get ID from the gridview and hold it in session
        if (e.CommandName == "RetrieveID")
        {
            //if (FCCstatus == null)
            //{

                string undoextrefno, ecarefno, effectivedate, createextrefno, custacno, custacbrn, approvedblklamt, userid, addtime;
                int crow;
                crow = Convert.ToInt32(e.CommandArgument.ToString());

                undoextrefno = GridView1.Rows[crow].Cells[1].Text;
                ecarefno = GridView1.Rows[crow].Cells[2].Text;
                effectivedate = GridView1.Rows[crow].Cells[3].Text;
                createextrefno = GridView1.Rows[crow].Cells[4].Text;
                custacno = GridView1.Rows[crow].Cells[5].Text;
                custacbrn = GridView1.Rows[crow].Cells[6].Text;
                approvedblklamt = GridView1.Rows[crow].Cells[7].Text;
                userid = GridView1.Rows[crow].Cells[8].Text;
                addtime = GridView1.Rows[crow].Cells[9].Text;

                FccCA.FCUBSCAService CAservice = new FccCA.FCUBSCAService();

                FccCA.CLOSEECABLK_FSFS_REQ CArequest = new FccCA.CLOSEECABLK_FSFS_REQ();
                FccCA.FCUBS_HEADERType header = new FccCA.FCUBS_HEADERType();
                FccCA.EcablkFullType body = new FccCA.EcablkFullType();

                FccCA.CLOSEECABLK_FSFS_REQFCUBS_BODY ecaDetails = new FccCA.CLOSEECABLK_FSFS_REQFCUBS_BODY();

                FccCA.CLOSEECABLK_FSFS_RES CAresponse = new FccCA.CLOSEECABLK_FSFS_RES();

                //Composite objects
                FccCA.FCUBS_HEADERTypePARAM param = new FccCA.FCUBS_HEADERTypePARAM();//PARAM inner tag
                FccCA.FCUBS_HEADERTypePARAM[] addl = new FccCA.FCUBS_HEADERTypePARAM[1];//ADDL tag

                FccCA.EcablkFullTypeEcaDetail ecadets = new FccCA.EcablkFullTypeEcaDetail();//EcaDetail Comp Obj
                FccCA.EcablkFullTypeEcaDetail[] ecaDets = new FccCA.EcablkFullTypeEcaDetail[1];

                header.SOURCE = "FCUBS";
                header.UBSCOMP = FccCA.UBSCOMPType.FCUBS;
                header.USERID = "SYSTEM";
                header.BRANCH = "055";
                header.SERVICE = "FCUBSCAService";
                header.OPERATION = "CloseEcablk";
                header.SOURCE_OPERATION = "CloseEcablk";
                header.DESTINATION = "FCUBS";

                param.NAME = "SERVERSTAT";
                param.VALUE = "HOST";
                addl[0] = param;
                header.ADDL = addl;

                //body
                body.UNDOEXTREFNO = undoextrefno;
                body.ECAREFNO = ecarefno;

                if (!string.IsNullOrEmpty(effectivedate))
                {
                    body.EFFECTIVEDATESpecified = true;
                    body.EFFECTIVEDATE = Convert.ToDateTime(effectivedate.Trim());
                }

                body.CREATEEXTREFNO = createextrefno;
                body.UPDASERRIFANYFAIL = "Y";
                body.BRN = "055";

                ecadets.BLKSTATUS = "K";
                ecadets.CUSTACNO = custacno;
                ecadets.CUSTACBRN = custacbrn;
                ecadets.PARTIALRELEASEALLOWED = "N";

                if (!string.IsNullOrEmpty(approvedblklamt))
                {
                    ecadets.APPROVEDBLKAMTSpecified = true;
                    ecadets.APPROVEDBLKAMT = Convert.ToDecimal(approvedblklamt.Trim());
                }

                ecaDets[0] = ecadets;
                body.EcaDetail = ecaDets;

                ecaDetails.EcaMasterFull = body;
                CArequest.FCUBS_BODY = ecaDetails;
                CArequest.FCUBS_HEADER = header;

                request = Serialize(CArequest);

                try
                {
                    FCCResponse = Serialize(CAresponse = CAservice.CloseEcablkFS(CArequest)); //v14 obj
                    response_message = parse_response(FCCResponse);

                    save_fcc_response(undoextrefno, response_message, request, FCCResponse);
                }
                catch (Exception ex)
                {
                    createEvent("Error posting " + undoextrefno + "to Flexcube" + ex.Message, "Error");
                    sendEmail(WebConfigurationManager.AppSettings["receivers"], "EnterprisePortal Error Posting " + undoextrefno + " to Flexcube", ex.Message);
                }                         

                lblMessage.Text = response_message.Substring(1);
           
            
        }
    }

    public static string Serialize(object dataToSerialize)
    {
        if (dataToSerialize == null) return null;

        using (System.IO.StringWriter stringwriter = new System.IO.StringWriter())
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(dataToSerialize.GetType());
            serializer.Serialize(stringwriter, dataToSerialize);
            return stringwriter.ToString();
        }
    }

    private static string parse_response(string Responce)
    {
        string ref_no = null;
        string status = null;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(Responce);
        XmlNodeList elemList = doc.GetElementsByTagName("FCCREF");
        int i = 0;
        for (i = 0; i < elemList.Count; i++)
        {
            ref_no = elemList[i].InnerXml;
            break;
        }

        status = "";
        XmlNodeList elemList2 = doc.GetElementsByTagName("WDESC");
        int i2 = 0;
        for (i2 = 0; i2 < elemList2.Count; i2++)
        {
            status = elemList2[i2].InnerXml;
        }

        if (string.IsNullOrEmpty(status))
        {
            XmlNodeList elemList3 = doc.GetElementsByTagName("EDESC");
            int i3 = 0;
            for (i3 = 0; i3 < elemList3.Count; i3++)
            {
                status = elemList3[i3].InnerXml;
                break;
            }
        }

        status = status.Replace("<![CDATA[", "");
        status = status.Replace("]]>", "");
        return ref_no + "|" + status;
    }
    private static void save_fcc_response(string transaction_ref, string fcc_response,string xml_request, string xml_response)
    {

        string sqlquery = "UPDATE tblBlockedFundsClearing SET serialNo = @srNo, FccRef =@fccRef, Status = @status, REQUEST_XML = @request_xml, RESPONSE_XML = @response_xml WHERE UNDOEXTREFNO = @srNo";
        string[] Trn = fcc_response.Split('|');
        string status = null;


        if (Trn[1].Equals("Successfully Closed And Authorized"))
        {
            status = "SUCCESS";
        }
        else
        {
            status = "FAILED";
        }
        using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["EPcon"].ConnectionString))
        {

            try
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(sqlquery, con))
                {
                    command.Parameters.AddWithValue("@srNo", transaction_ref);
                    command.Parameters.AddWithValue("@fccRef", Trn[0]);
                    command.Parameters.AddWithValue("@status", status);
                    command.Parameters.AddWithValue("@request_xml", xml_request);
                    command.Parameters.AddWithValue("@response_xml", xml_response);

                    command.ExecuteNonQuery();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                createEvent(ex.ToString() + " when saving Flex response " + transaction_ref, "Error");
                sendEmail(WebConfigurationManager.AppSettings["recepient"], "EnterprisePortal SAVING FLEX RESPONSE Failing", "EnterprisePortal Failing,error: " + ex.ToString());

            }

        }
    }

    private static void createEvent(string sEvent, string sEventType)
    {
        string sSource;
        string sLog;


        sSource = "EnterprisePortal";
        sLog = "EnterprisePortal_Log";

        if (!EventLog.SourceExists(sSource))
            EventLog.CreateEventSource(sSource, sLog);

        if (sEventType.Equals("Error"))
        {
            EventLog.WriteEntry(sSource, sEvent,
            EventLogEntryType.Error);
        }
        else if (sEventType.Equals("Warning"))
        {
            EventLog.WriteEntry(sSource, sEvent,
            EventLogEntryType.Warning);
        }
        else if (sEventType.Equals("Success"))
        {
            EventLog.WriteEntry(sSource, sEvent,
            EventLogEntryType.Information);
        }
        else if (sEventType.Equals("Audit"))
        {
            EventLog.WriteEntry(sSource, sEvent,
            EventLogEntryType.SuccessAudit);
        }

    }

    private static void sendEmail(string recipient, string subject, string message)
    {

        MailMessage Message = new MailMessage();
        Message.From = new MailAddress(WebConfigurationManager.AppSettings["EmailSender"]);
        Message.To.Add(new MailAddress(recipient));
        Message.Subject = subject;
        Message.Body = message;
        SmtpClient client = new SmtpClient(WebConfigurationManager.AppSettings["SMTPServer"], int.Parse(WebConfigurationManager.AppSettings["SMTPPort"]));
        try
        {
            client.Send(Message);

        }
        catch (Exception ex)
        {
            createEvent("Error sending EnterprisePortal Challenge :" + ex.ToString(), "Error");
        }
    }

    //sorting grid
    protected void grdCause_Sorting(object sender, GridViewSortEventArgs e)
    {
        string sortExpression = e.SortExpression;

        if (GridViewSortDirection == SortDirection.Ascending)
        {
            GridViewSortDirection = SortDirection.Descending;
            SortGridView(sortExpression, " DESC");
        }
        else
        {
            GridViewSortDirection = SortDirection.Ascending;
            SortGridView(sortExpression, " ASC");
        }


    }

    private void SortGridView(string sortExpression, string direction)
    {
        using (SqlConnection cnn = new SqlConnection(WebConfigurationManager.ConnectionStrings["EPcon"].ConnectionString))
        {
            cnn.Open();
            SqlDataAdapter sadpt = new SqlDataAdapter(WebConfigurationManager.AppSettings["GetBlockedFunds"], cnn);

            DataTable DT = new DataTable();
            sadpt.Fill(DT);
            DataView dv = new DataView(DT);
            dv.Sort = sortExpression + direction;

            GridView1.DataSource = dv;
            GridView1.DataBind();
            cnn.Close();
        }

    }

    public SortDirection GridViewSortDirection
    {
        get
        {
            if (ViewState["sortDirection"] == null)
                ViewState["sortDirection"] = SortDirection.Ascending;

            return (SortDirection)ViewState["sortDirection"];
        }
        set
        {
            ViewState["sortDirection"] = value;
        }
    }
    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {

        GridView1.PageIndex = e.NewPageIndex;
        GridView1.DataBind();

    }
   
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(txtAccNo.Text))
        {
            lblMessage.Text = "Enter CUSTACNO!";

        }
        else{
            try
            {
                using (SqlConnection cnn = new SqlConnection(WebConfigurationManager.ConnectionStrings["EPcon"].ConnectionString))
                {
                    cnn.Open();
                    SqlDataAdapter sadpt = new SqlDataAdapter(WebConfigurationManager.AppSettings["GetFilterExBlockedFunds"].Replace("@CUSTACNO", txtAccNo.Text), cnn);
                    DataTable DT = new DataTable();
                    //DataRow rw;
                    sadpt.Fill(DT);
                    requestID = (from DataRow dr in DT.Rows select (String)dr["CUSTACNO"]).FirstOrDefault();

                    try { FCCstatus = (from DataRow dr in DT.Rows select (String)dr["Status"]).FirstOrDefault(); }
                    catch (InvalidCastException ex) {FCCstatus = null;}
               
                    if (DT.Rows.Count > 0)
                    {
                        GridView1.DataSource = DT;
                        GridView1.DataBind();
                        lblMessage.Text = "";
                    }
                    else
                    {
                        GridView1.DataSource = DT;
                        GridView1.DataBind();
                        lblMessage.Text = "Record not found!";

                    }
                    cnn.Close();

                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
        }
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void txtAccNo_TextChanged(object sender, EventArgs e)
    {

    }
}