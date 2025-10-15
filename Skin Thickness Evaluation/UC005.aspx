<%@ Page Title="SPC Analysis Filtering" Language="C#" MasterPageFile="~/SiteSub.Master"
    AutoEventWireup="true" CodeBehind="UC005.aspx.cs" Inherits="Skin_Thickness_Evaluation.UC093" %>

<%@ Register Src="Components/UC_SPCReport.ascx" TagName="UC_SPCReport" TagPrefix="uc1" %>
<%@ Register TagPrefix="filter" TagName="Scan_Profile" Src="~/Components/Filter/Filter_ScanProfile.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="LeftBar" runat="server">
    <h3>
        <a href="#">SPC Options</a></h3>
    <div>
        <filter:Scan_Profile ID="Scan_Profile1" runat="server" />
        <fieldset class="ui-corner-all">
            <legend>Select a sample...</legend>
            <ul>
                <li>
                    <input name="BladeSampleSize" id="BladesTop" type="radio" value="10" checked="checked" />
                    Last 10 blades</li>
                <li>
                    <input name="BladeSampleSize" id="BladesTop50" type="radio" value="50" />
                    Last 50 blades</li>
                <li>
                    <input name="BladeSampleSize" id="BladesTop100" type="radio" value="100" />
                    Last 100 blades</li>
                <li>
                    <input name="BladeSampleSize" id="BladesAll" type="radio" value="All" />
                    All blades</li>
                <li>
                    <input name="BladeSampleSize" id="BladeDates" type="radio" value="Date" />
                    Between these dates
                    <div id="showBladeDates">
                        <div style="float: right; width: 90%; text-align: right;">
                            <label for="Text1">
                                From</label>
                            <input type="text" name="DateStart" id="DateStart" class="datepicker" />
                        </div>
                        <div style="float: right; width: 90%; text-align: right;">
                            <label for="Text2">
                                To</label>
                            <input type="text" name="DateStop" id="DateStop" class="datepicker" />
                        </div>
                    </div>
                </li>
            </ul>
        </fieldset>
        <fieldset class="ui-corner-all">
            <legend>Select a sample...</legend>
            <div style="position: relative; width: 90%;">
                <select id="ScanForm" style="width: 140px; float: right;">
                    <option value="180">Inside</option>
                    <option value="0">Outside</option>
                </select>
                <label for="ScanForm">
                    Blade from</label>
            </div>
            <div style="position: relative; width: 90%;">
                <select id="ScanRow" style="width:  140px; float: right;">
                </select>
                <label for="ScanRow">
                    Section Y</label>
            </div>
            <div style="position: relative; width: 90%;">
                <select id="ScanCol" style="width:  140px; float: right;">
                </select>
                <label for="ScanCol">
                    Chord X</label>
            </div>
        </fieldset>
        <script type="text/javascript">
            function getRowList() {
                var ProfileID = $('#<%=Scan_Profile1.ScanProfile.ClientID%>').val();
                var Form = $('#ScanForm').val();

                $.ajax({
                    type: 'POST',
                    url: 'API/AJAX_Requests.asmx/Select_Row_List',
                    data: '{ profile: "' + ProfileID + '", Form:  "' + Form + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: 'json',
                    success: function (msg) {
                        $('#ScanRow').empty();

                        var c = eval(msg.d);
                        for (var i in c) {
                            //$('#ScanRow').append(new Option(c[i], c[i]));
                            $("<option>").attr("value", c[i]).text(c[i]).appendTo("#ScanRow");
                        }

                        getColList();
                    }
                });
            }

            function getColList() {
                var ProfileID = $('#<%=Scan_Profile1.ScanProfile.ClientID%>').val();
                var Form = $('#ScanForm').val();
                var Row = $('#ScanRow').val();

                $.ajax({
                    type: 'POST',
                    url: 'API/AJAX_Requests.asmx/Select_Col_List',
                    data: '{ profile: "' + ProfileID + '", Form: "' + Form + '", Row:  "' + Row + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: 'json',
                    success: function (msg) {
                        $('#ScanCol').empty();

                        var c = eval(msg.d);
                        for (var i in c) {
                            //$('#ScanCol').append(new Option(c[i], c[i]));
                            $("<option>").attr("value", c[i]).text(c[i]).appendTo("#ScanCol");
                        }
                    }
                });
            }

            $(document).ready(function () {
                $('#<%=Scan_Profile1.ScanProfile.ClientID%>').change(getRowList);
                $('#ScanForm').change(getRowList);
                $('#ScanRow').change(getColList);

                getRowList();

                $('input:radio').change(function () {
                    if ($('#BladeDates').is(':checked')) {
                        $('#showBladeDates').show('slow');
                    } else {
                        $('#showBladeDates').hide();
                    }
                });
                $('input:radio').change();
            });
        </script>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:UC_SPCReport ID="UC_SPCReport1" runat="server" width="925px" height="725px" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
    <script type="text/javascript">
        function updateChart() {
            var ChartOptions = {
                Profile: $('#<%=Scan_Profile1.ScanProfile.ClientID%>').val(),
                Form: $('#ScanForm').val(),
                Row: $('#ScanRow').val(),
                Col: $('#ScanCol').val(),
                SampleSize: $("input[name='BladeSampleSize']:checked").val(),
                DateStart: $('#DateStart').val(),
                DateStop: $('#DateStop').val()
            };

            updateChart_Image(ChartOptions);
            return false;
        }
    </script>
    <a href="#" class="button" onclick="updateChart();">Calculate</a>
</asp:Content>
