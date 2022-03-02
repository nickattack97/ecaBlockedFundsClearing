<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="pgBlockedFundsClearing.aspx.cs" Inherits="pgBlockedFundsClearing" %>

<asp:Content id="Content1" contentplaceholderid="head" runat="Server">

    <script type="text/javascript">

        function DisableBackButton() {
            window.history.forward()
        }
        DisableBackButton();
        window.onload = DisableBackButton;
        window.onpageshow = function (evt) { if (evt.persisted) DisableBackButton() }
        window.onunload = function () { void (0) }


     </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<section class="content">
      <div class="container-fluid">
              
        <div class="col-md-12">
          <div class="box box-primary">
<h5 class="text-center title-2"><asp:Label ID="lblUsername" ForeColor="Green" runat="server" Visible="False"></asp:Label></h5>
            <!-- /.box-header -->
            <div class="box-body no-padding">
          <div class="box-body">
            <div class="row">
               <asp:Panel ID="AuthorisationPanel" runat="server" Visible="true">
                <div class="col-xs-9">
                    <asp:TextBox ID="txtAccNo" placeholder="Enter CUSTACNO here..." class="form-control" runat="server" OnTextChanged="txtAccNo_TextChanged"></asp:TextBox>
                </div>                    
                   <!-- /.col -->
                <div class="col-xs-2">
                <asp:Button ID="btnSearch" class="btn btn-primary btn-block btn-flat" runat="server" Text="Search" OnClick="btnSearch_Click"/>
                </div>             
              </asp:Panel>
             </div>
             </div>


            <div class="box-body">               
                <asp:GridView ID="GridView1" class="table table-bordered" runat="server" AutoGenerateColumns="False" AllowPaging="True" OnRowCommand="GridView1_RowCommand" AllowSorting="True" onsorting="grdCause_Sorting" OnPageIndexChanging="GridView1_PageIndexChanging " PageSize="7" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" >
                    <Columns>
                        <asp:ButtonField CommandName="RetrieveID" Text="PROCESS" />
                        <asp:BoundField DataField="UNDOEXTREFNO" HeaderText="UNDOEXTREFNO" SortExpression="UNDOEXTREFNO" />
                        <asp:BoundField DataField="ECAREFNO" HeaderText="ECAREFNO" SortExpression="ECAREFNO" />
                        <asp:BoundField DataField="EFFECTIVEDATE" HeaderText="EFFECTIVEDATE" SortExpression="EFFECTIVEDATE" />
                        <asp:BoundField DataField="CREATEEXTREFNO" HeaderText="CREATEEXTREFNO" SortExpression="CREATEEXTREFNO" />
                        <asp:BoundField DataField="CUSTACNO" HeaderText="CUSTACNO" SortExpression="CUSTACNO" />
                        <asp:BoundField DataField="CUSTACBRN" HeaderText="CUSTACBRN" SortExpression="CUSTACBRN" />
                        <asp:BoundField DataField="APPROVEDBLKAMT" HeaderText="APPROVEDBLKAMT" SortExpression="APPROVEDBLKAMT" />
                        <asp:BoundField DataField="FCCREF" HeaderText="FCCREF" SortExpression="FCCREF" />
                        <asp:BoundField DataField="STATUS" HeaderText="STATUS" SortExpression="STATUS" />
                    </Columns>
                </asp:GridView>

								




            </div>
                <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
            </div>
          </div>
          <!-- /. box -->
        </div>
             </div>
</section>
</asp:Content> 