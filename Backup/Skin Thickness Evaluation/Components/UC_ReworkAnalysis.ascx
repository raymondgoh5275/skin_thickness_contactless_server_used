<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UC_ReworkAnalysis.ascx.cs" Inherits="Skin_Thickness_Evaluation.Components.UC_ReworkAnalysis" %>

<div style="float:right; width:150px; padding:0px;">

    <fieldset class="ui-corner-all" style=" text-align:center;" id="RequestInfo">
        <p>&nbsp;<br />&nbsp;<br />&nbsp;<br />&nbsp;</p>
    </fieldset>

    <!-- fieldset class="ui-corner-all" style=" text-align:center; ">
        <p>Section&nbsp;&nbsp;-</p>
        <p>Chord&nbsp;&nbsp;&nbsp;&nbsp;-</p>    
        <p>Value&nbsp;&nbsp;&nbsp;&nbsp;-</p>
    </fieldset -->

    <fieldset class="ui-corner-all" style=" text-align:center; ">
        <table cellspacing="0" style=" width:100%; border-spacing:0px; border-collapse:separate; text-align:center;">
        <tr>    <td style=" background-color:#0ff; padding:2px; " id="Scale00">&gt; &gt; 2.20</td> </tr>
        <tr>    <td style=" background-color:#808; padding:2px; " id="Scale01">1.80 to 2.20</td> </tr>
        <tr>    <td style=" background-color:#800; padding:2px; " id="Scale02">1.40 to 1.80</td> </tr>
        <tr>    <td style=" background-color:#088; padding:2px; " id="Scale03">1.00 to 1.40</td> </tr>
        <tr>    <td style=" background-color:#080; padding:2px; " id="Scale04">0.60 to 1.00</td> </tr>
        <tr>    <td style=" background-color:#008; padding:2px; " id="Scale05">0.20 to 0.60</td> </tr>
        <tr>    <td style=" background-color:#f00; padding:2px; " id="Scale06">-0.20 to 0.20</td> </tr>
        <tr>    <td style=" background-color:#f00; padding:2px; " id="Scale07">-0.20 to -0.60</td> </tr>
        <tr>    <td style=" background-color:#f00; padding:2px; " id="Scale08">-0.60 to -1.00</td> </tr>
        <tr>    <td style=" background-color:#f00; padding:2px; " id="Scale09">-1.00 to -1.40</td> </tr>
        <tr>    <td style=" background-color:#f00; padding:2px; " id="Scale10">-1.40 to -1.80</td> </tr>
        <tr>    <td style=" background-color:#f00; padding:2px; " id="Scale11">-1.80 to -2.20</td> </tr>
        <tr>    <td style=" background-color:#f00; padding:2px; " id="Scale12">&lt; &lt; -2.20</td> </tr>                
        </table>
        <label for="scaleInput">Scale</label><input type="text" id="scaleInput" name="scaleInput" size="5" value="0.2" /><br />
        <label for="scaleOffset">Offset</label><input type="text" id="scaleOffset" name="scaleOffset" size="5" value="0" /><br />
        
        <!--input type="checkbox" id="traficLights" name="traficLights" /><label for="traficLights">Traffic lights</label><br />
        <input type="checkbox" id="centreLine" name="centreLine" /><label for="centreLine">Centre line</label-->
    </fieldset>
</div>
<div style="float: left; width: 80%; height: 95%; min-width: 640px; min-height: 480px;
    padding: 0px;">
    <table class="BladeDisplay">
        <thead>
            <tr>
                <th colspan="2">Outside form</th>
                <td>&nbsp;</td>
                <th colspan="2">Inside form</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <th rowspan="2">Leading edge</th>
                <td rowspan="3" id="ReworkAnalysisOutsideForm" style="width: 260px; height: 470px;"></td>
                <th>ROOT</th>
                <td rowspan="3" id="ReworkAnalysisInsideForm" style="width: 260px; height: 470px;"></td>
                <th rowspan="2">Leading edge</th>
            </tr>
            <tr>
                <th style="vertical-align: bottom; height: 150px;">Trailing edge</th>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <th style="vertical-align: bottom;">TIP</th>
                <td>&nbsp;</td>
            </tr>
        </tbody>
    </table>
    <script type="text/javascript">
        function RequestImagesOut(strBladeID) {
            $('#ReworkAnalysisOutsideForm').removeClass('warning');
            $('#ReworkAnalysisOutsideForm').addClass('loading');
            $('#ReworkAnalysisOutsideForm').empty();

            var img = new Image();

            img.src = 'API/ImageHandler.ashx?I=ReworkAnalysisBladeImage&BladeID=' + strBladeID + '&Form=0&Scale=' + $('#scaleInput').val() + '&Offset=' + $('#scaleOffset').val();

            $('#ReworkAnalysisOutsideForm').append(img);
            $(img).fadeIn();
            $('#ReworkAnalysisOutsideForm').removeClass('loading');
        }
        function RequestImagesIn(strBladeID) {

            $('#ReworkAnalysisInsideForm').removeClass('warning');
            $('#ReworkAnalysisInsideForm').addClass('loading');
            $('#ReworkAnalysisInsideForm').empty();

            var img = new Image();

            img.src = 'API/ImageHandler.ashx?I=ReworkAnalysisBladeImage&BladeID=' + strBladeID + '&Form=180&Scale=' + $('#scaleInput').val() + '&Offset=' + $('#scaleOffset').val();

            $('#ReworkAnalysisInsideForm').append(img);
            $(img).fadeIn();
            $('#ReworkAnalysisInsideForm').removeClass('loading');
        }

        function RequestBladeInfo(Profile, BladeText) {
            $("#RequestInfo").empty();
            var Scale = $('#scaleInput').val();
            var Offset = $('#scaleOffset').val();

            // <p><strong>T900</strong><br />01K85383</p>
            var DisplayText = "<p><strong>" + Profile + "</strong></p>";

            var temp = BladeText.split(',');
            //<p>RGN10373<br />FROM MINIMUM<br />DATE OF SCAN<br />2003-10-29 09:53:42</p>
            DisplayText += "<p>" + temp[0] + "<br />FROM MINIMUM<br />DATE OF SCAN<br />";
            var temp2 = temp[1].split("(");
            DisplayText += temp2[0] + "</p>";

            $("#RequestInfo").html(DisplayText);

            // Scale Key
            var ScaleList = {};
            var top = parseFloat(Offset) + parseFloat((parseFloat(Scale) / 2) + (parseFloat(Scale) * 5));
            for (var i = 0; i < 12; i++)
            {
                ScaleList[i] = top - (Scale * i);
            }

            $('#Scale00').html("&gt; &gt; " + ScaleList[0].toFixed(2));
            $('#Scale01').html(ScaleList[1].toFixed(2) + " to " + ScaleList[0].toFixed(2));
            $('#Scale02').html(ScaleList[2].toFixed(2) + " to " + ScaleList[1].toFixed(2));
            $('#Scale03').html(ScaleList[3].toFixed(2) + " to " + ScaleList[2].toFixed(2));
            $('#Scale04').html(ScaleList[4].toFixed(2) + " to " + ScaleList[3].toFixed(2));
            $('#Scale05').html(ScaleList[5].toFixed(2) + " to " + ScaleList[4].toFixed(2));
            $('#Scale06').html(ScaleList[6].toFixed(2) + " to " + ScaleList[5].toFixed(2));
            $('#Scale07').html(ScaleList[6].toFixed(2) + " to " + ScaleList[7].toFixed(2));
            $('#Scale08').html(ScaleList[7].toFixed(2) + " to " + ScaleList[8].toFixed(2));
            $('#Scale09').html(ScaleList[8].toFixed(2) + " to " + ScaleList[9].toFixed(2));
            $('#Scale10').html(ScaleList[9].toFixed(2) + " to " + ScaleList[10].toFixed(2));
            $('#Scale11').html(ScaleList[10].toFixed(2) + " to " + ScaleList[11].toFixed(2));
            $('#Scale12').html("&lt; &lt; " + ScaleList[11].toFixed(2));
        }
    </script>
</div>