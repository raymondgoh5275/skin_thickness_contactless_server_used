<%@ Page Title="Metal Removal Filtering" Language="C#" MasterPageFile="~/SiteSub.Master" AutoEventWireup="true" CodeBehind="UC008.aspx.cs" Inherits="Skin_Thickness_Evaluation.UC096" %>

<%@ Register TagPrefix="STE" TagName="MetalRemoval" Src="~/Components/UC_MetalRemoval.ascx" %>
<%@ Register TagPrefix="filter" TagName="BladeList" Src="~/Components/Filter/Filter_BladeList.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="LeftBar" runat="server">
        <h3>
        <a href="#">Export Options</a></h3>
    <div>
        <filter:BladeList ID="BladeList1" runat="server" MultiSelect="false" />

        <fieldset class="ui-corner-all">
            <p style="margin:3px;"><strong>Metal removal = Scan A - Scan B</strong></p>
            <script type="text/javascript">
                var strCodeA, strCodeB;

                function ButScanA_OnClick() {
                    $('#ScanAid').val($('#BladeList option:selected').val());
                    var BladeDesc = $('#BladeList option:selected').text();
                    strCodeA = BladeDesc.substring(0, BladeDesc.indexOf(','));
                    var strDate = BladeDesc.substring(BladeDesc.indexOf(',')+1, BladeDesc.indexOf('('));
                    var strDesc = 'Scan A = ' + strCodeA + ' on ' + jQuery.trim(strDate);
                    $('#labelScanA').text(strDesc);

                    return false;
                }

                function ButScanB_OnClick() {
                    $('#ScanBid').val($('#BladeList option:selected').val());
                    var BladeDesc = $('#BladeList option:selected').text();
                    strCodeB = BladeDesc.substring(0, BladeDesc.indexOf(','));
                    var strDate = BladeDesc.substring(BladeDesc.indexOf(',') + 1, BladeDesc.indexOf('('));
                    var strDesc = 'Scan B = ' + strCodeB + ' on ' + jQuery.trim(strDate);
                    $('#labelScanB').text(strDesc);

                    return false;
                }
            </script>
            <div>
                <input type="hidden" id="ScanAid" />
                <input type="button" id="ButScanA" value="Use" style="float:right;" onclick="ButScanA_OnClick()" />
                <label for="Button1" id="labelScanA">Scan A = </label>
            </div>
            <div>
                <input type="hidden" id="ScanBid" />
                <input type="button" id="ButScanB" value="Use" style="float:right;" onclick="ButScanB_OnClick()" />
                <label for="Button2" id="labelScanB">Scan B = </label>
            </div>
        </fieldset>

        <div class="clear"></div>
    </div>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <STE:MetalRemoval id="MetalRemoval01" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
    <script type="text/javascript">
        function OnClick_Calculate() {
            var ScanAid = $('#ScanAid').val();
            var ScanBid = $('#ScanBid').val();

            if ((ScanAid == '') || (ScanBid == '')) {
                alert('You must select both blades.');
                return false;
            }

            RequestImagesOut(ScanAid, ScanBid);
            RequestImagesIn(ScanAid, ScanBid);

            RequestBladeInfo(SelectedProfileText, strCodeA, strCodeB);
            return false;
        }
    </script>
    <a href="#" class="button" onclick="OnClick_Calculate();">Calculate</a>
</asp:Content>