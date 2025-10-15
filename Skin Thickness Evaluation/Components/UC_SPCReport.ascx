<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UC_SPCReport.ascx.cs" Inherits="Skin_Thickness_Evaluation.Components.UC_SPCReport" %>

<div id="SPCReport" runat="server"></div>

<script type="text/javascript">
    var ReportArea;

    function updateChart_Image(postData) {
        ReportArea = $('#<%=SPCReport.ClientID %>');
        ReportArea.removeClass('warning');
        ReportArea.addClass('loading');
        ReportArea.empty();

        var img = new Image();

        img.src = 'API/ImageHandler.ashx?I=Create_SPCReport&Profile=' + postData.Profile + '&Form=' + postData.Form + '&Row=' + postData.Row + '&Col=' + postData.Col + '&SampleSize=' + postData.SampleSize + '&DateStart=' + postData.DateStart + '&DateStop=' + postData.DateStop;

        ReportArea.append(img);
        $(img).fadeIn();
        ReportArea.removeClass('loading');
      }
</script>