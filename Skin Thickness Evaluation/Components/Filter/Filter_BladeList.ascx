<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Filter_BladeList.ascx.cs"
    Inherits="Skin_Thickness_Evaluation.Components.Filter.Filter_BladeList" %>
<%@ Register TagPrefix="filter" TagName="Scan_Profile" Src="~/Components/Filter/Filter_ScanProfile.ascx" %>
<filter:Scan_Profile ID="Scan_Profile1" runat="server" />
<fieldset class="ui-corner-all" style="">
    <legend>Select blades...</legend>
    <select id="BladeList" multiple="multiple" style="overflow: auto; height: 180px; width: 100%;"></select>
    <div id="divSelectedBlade" runat="server">
        <label for="Text1">
            Selected blade number</label>
        <input type="text" name="Text1" id="SelectedBlade" style="width: 85px; float: right;" />
        <script language="javascript" type="text/javascript">
            function SelectBlade() {
                $('#SelectedBlade').val($('#BladeList option:selected').text().substring(0, $('#BladeList option:selected').text().indexOf(',')));
            }

            $(document).ready(function () {
                $('#BladeList').change(SelectBlade);
            });
        </script>
    </div>
    <input type="hidden" id="hidBladeIDs" />
    <input type="hidden" id="HidBladeCount" />

    <script language="javascript" type="text/javascript">
        var BladeMultiSelect = <%=MultiSelect.ToString().ToLower() %>;
        function SelectBlades() {
            $('#hidBladeIDs').val("");
            var strSelectedBlaidIDs = ""
            var intCountSelectedBlaid = 0;
            if (BladeMultiSelect) {
                $('#BladeList option:selected').each(function () {
                    strSelectedBlaidIDs += $(this).val() + ",";
                    intCountSelectedBlaid++;
                });
            } else {
                strSelectedBlaidIDs = $('#BladeList option:selected').val() + ",";
                intCountSelectedBlaid++;
            }
            $('#hidBladeIDs').val(strSelectedBlaidIDs.substring(0, strSelectedBlaidIDs.length - 1)).change(); ;
            $('#HidBladeCount').val(intCountSelectedBlaid).change(); ;
        }

        $(document).ready(function () {
            $('#BladeList').change(SelectBlades);
        });
    </script>

</fieldset>
<script language="javascript" type="text/javascript">
    var SelectedProfile;
    var SelectedProfileText;

    function getBladeList() {
        SelectedProfile = $('#<%=Scan_Profile1.ScanProfile.ClientID%> option:selected').val();
        SelectedProfileText = $('#<%=Scan_Profile1.ScanProfile.ClientID%> option:selected').text()
        $.ajax({
            type: 'POST',
            url: 'API/AJAX_Requests.asmx/Select_Blade_List',
            data: '{ profile: "' + SelectedProfile + '" }',
            dataType: 'jason',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                $('#BladeList').empty();
                var c = eval(msg.d);
                for (var i in c) {
                    $("<option>").attr("value", c[i][0]).text(c[i][1]).appendTo("#BladeList");
                    //$('#BladeList').append(new Option(c[i][1], c[i][0]));
                }
            }
        });
    }
    $(document).ready(function () {
        $('#<%=Scan_Profile1.ScanProfile.ClientID%>').change(getBladeList);
        getBladeList();
    });
</script>
