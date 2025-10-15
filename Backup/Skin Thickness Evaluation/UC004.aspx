<%@ Page Title="Contour Plotting Filtering and calculation" Language="C#"
    MasterPageFile="~/SiteSub.Master" AutoEventWireup="true" CodeBehind="UC004.aspx.cs"
    Inherits="Skin_Thickness_Evaluation.UC092" %>

<%@ Register TagPrefix="STE" TagName="ContourPlotting" Src="~/Components/UC_ContourPlotting.ascx" %>
<%@ Register TagPrefix="filter" TagName="BladeList" Src="~/Components/Filter/Filter_BladeList.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LeftBar" runat="server">
    <h3>
        <a href="#">Sample Selection&nbsp;(<span id="SampleCount">0</span>)</a></h3>
    <div>
        <filter:BladeList ID="BladeList1" runat="server" MultiSelect="true" />
        <script language="javascript" type="text/javascript">
            function bladeCountChanged() {
                $('#SampleCount').text($(this).val());
            }

            $(document).ready(function () {
                $('#HidBladeCount').change(bladeCountChanged);
            });
        </script>
    </div>
    <h3>
        <a href="#">Calculation Selection</a></h3>
    <div>
        <!--div style="position: relative; width: 100%;">
            <input id="Checkbox1" type="checkbox" />&nbsp;Export the results to a worksheet
        </div -->
        <fieldset class="ui-corner-all">
            <legend>Calculate the sample...</legend>
            <ul>
                <li><input name="calculate" id="calcAvg" type="radio" value="0" checked="checked" /><label for="calcAvg">Average</label></li>
                <li><input name="calculate" id="calcStdev" type="radio" value="1" /><label for="calcStdev">Standard deviation</label></li>
                <li><input name="calculate" id="calcSpread" type="radio" value="2" /><label for="calcSpread">Spread / max - min</label></li>
                <li><input name="calculate" id="calcZusl" type="radio" value="3" /><label for="calcZusl">Z usl</label></li>
                <li><input name="calculate" id="calcZlsl" type="radio" value="4" /><label for="calcZlsl">Z lsl</label></li>
                <li><input name="calculate" id="calcZmin" type="radio" value="5" /><label for="calcZmin">Z min</label></li>
                <li><input name="calculate" id="calcPp" type="radio" value="6" /><label for="calcPp">Pp</label></li>
                <li><input name="calculate" id="calcPpk" type="radio" value="7" /><label for="calcPpk">Ppk</label></li>
            </ul>
        </fieldset>
        <fieldset class="ui-corner-all" id="compareTo">
            <legend>and compare the results to..</legend>
            <ul>
                <li><input name="compareTo" id="MaxTol" type="radio" value="0" /><label for="MaxTol">Maximum tolerance</label></li>
                <li><input name="compareTo" id="NorTol" type="radio" value="1" checked="checked" /><label for="NorTol">Normal tolerance</label></li>
                <li><input name="compareTo" id="MinTol" type="radio" value="2" /><label for="MinTol">Minimum tolerance</label></li>
                <li><input name="compareTo" id="NoTol" type="radio" value="3" /><label for="NoTol">No tolerances</label></li>
            </ul>
        </fieldset>
        <script type="text/javascript">
            $('input:radio[name="calculate"]').change(function () {
                if ($('#calcAvg').is(':checked')) {
                    $('#compareTo').show('slow');
                } else {
                    $('#compareTo').hide('slow');
                }
            });
            $('input:radio').change();
        </script>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <STE:ContourPlotting ID="ContourPlotting01" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="server">
    <script type="text/javascript">
        function OnClick_Calculate() {
            var Profile = SelectedProfileText;
            var BladeCount = $('#SampleCount').text();
            var BladeIDs = $('#hidBladeIDs').val();
            var Calculate = $('input:radio[name=calculate]:checked').val();
            var CompareTo = $('input:radio[name=compareTo]:checked').val();

            if (BladeIDs == '') {
                alert('you must select at least one blade.');
                return false;
            }
            RequestImagesOut(BladeIDs, Calculate, CompareTo);
            RequestImagesIn(BladeIDs, Calculate, CompareTo);
            UpdateRequestInfo(Profile, BladeCount, Calculate, CompareTo);
            return false;
        }
    </script>
    <a href="#" class="button" onclick="OnClick_Calculate();">Calculate</a>
</asp:Content>
