<%@ Page Title="Cavity Reports Analysis Filtering" Language="C#" MasterPageFile="~/SiteSub.Master"
    AutoEventWireup="true" CodeBehind="UC009.aspx.cs" Inherits="Skin_Thickness_Evaluation.UC101" %>

<%@ Register TagPrefix="filter" TagName="Scan_Profile" Src="~/Components/Filter/Filter_ScanProfile.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftBar" runat="server">
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
                <select id="ScanRow" style="width: 140px; float: right;">
                </select>
                <label for="ScanRow">
                    Section Y</label>
            </div>
        </fieldset>
        <script type="text/javascript">
            function getRowList() {
                var ProfileID = $('#<%=Scan_Profile1.ScanProfile.ClientID%>').val();
                var Form = 0;

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
                    }
                });
            }

            $(document).ready(function () {
                $('#<%=Scan_Profile1.ScanProfile.ClientID%>').change(getRowList);

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
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <fieldset class="ui-corner-all" style="float: left; width: 47%; min-height: 300px;">
        <legend>Inside form leading edge delta</legend>
        <div id="InsideSPCReport" style="height: 92%;">
        </div>
    </fieldset>
    <fieldset class="ui-corner-all" style="float: left; width: 47%; min-height: 300px;">
        <legend>Outside form leading edge delta</legend>
        <div id="OutsideSPCReport" style="height: 92%;">
        </div>
    </fieldset>
    <fieldset class="ui-corner-all" style="float: left;">
        <style>
            #tabdata
            {
                margin: 5px;
            }
            #tabdata td, #tabdata th
            {
                border: 1px solid silver;
                padding: 3px;
            }
            #tabdata th
            {
                background-color: gray;
            }
            #tabdata td
            {
                text-align: center;
                padding: 5px;
            }
        </style>
        <legend>leading edge delta tabular report</legend>
        <div id="tabdata">
        </div>
    </fieldset>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FooterContent" runat="server">
    <script type="text/javascript">
        function updateChart_Image(postData, ReportArea) {
            ReportArea.removeClass('warning');
            ReportArea.addClass('loading');
            ReportArea.empty();

            var img = new Image();

            img.src = 'API/ImageHandler.ashx?I=Create_DeltaSPCReport&Profile=' + postData.Profile + '&Form=' + postData.Form + '&Row=' + postData.Row + '&SampleSize=' + postData.SampleSize + '&DateStart=' + postData.DateStart + '&DateStop=' + postData.DateStop;

            ReportArea.append(img);
            $(img).fadeIn();
            ReportArea.removeClass('loading');
        }

        function updateChart_table(postData) {
            $("#tabdata").empty();

            $.ajax({
                type: 'POST',
                url: 'API/AJAX_Requests.asmx/Create_DeltaReport',
                data: JSON.stringify(postData),
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                success: function (msg) {
                    $("#tabdata").html(msg.d);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $("#tabdata").html('<tr><td><strong>' + textStatus + ':</strong>' + jqXHR.statusText + '.</td></tr>');
                }
            });
        }

        function updateCharts() {
            var ChartOptions = {
                Profile: $('#<%=Scan_Profile1.ScanProfile.ClientID%>').val(),
                Row: $('#ScanRow').val(),
                SampleSize: $("input[name='BladeSampleSize']:checked").val(),
                DateStart: $('#DateStart').val(),
                DateStop: $('#DateStop').val(),
                Form: 0
            };

            updateChart_Image(ChartOptions, $("#InsideSPCReport"));
            ChartOptions.Form = 180;
            updateChart_Image(ChartOptions, $("#OutsideSPCReport"));

            updateChart_table(ChartOptions);

            return false;
        }

        function DownloadCSV() {
            var ChartOptions = {
                Profile: $('#<%=Scan_Profile1.ScanProfile.ClientID%>').val(),
                Row: $('#ScanRow').val(),
                SampleSize: $("input[name='BladeSampleSize']:checked").val(),
                DateStart: $('#DateStart').val(),
                DateStop: $('#DateStop').val()
            };

            window.open('API/CSV.aspx?M=CRAF&id=' + ChartOptions.Profile + '&Row=' + ChartOptions.Row + '&SampleSize=' + ChartOptions.SampleSize + '&DateStart=' + ChartOptions.DateStart + '&DateStop=' + ChartOptions.DateStop);

            return false;
        }
    </script>
    <a href="#" class="button" onclick="DownloadCSV();">Download</a> <a href="#" class="button"
        onclick="updateCharts();">Calculate</a>
</asp:Content>
