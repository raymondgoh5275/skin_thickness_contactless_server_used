<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UC_ContourPlotting.ascx.cs"
    Inherits="Skin_Thickness_Evaluation.Components.UC_ContourPlotting" %>
<div style="float: right; width: 150px; padding: 0px;">
    <fieldset class="ui-corner-all" style="text-align: center;" id="RequestInfo">
        <p>
            <strong>&nbsp;</strong><br />
            &nbsp;</p>
        <p>
            &nbsp;<br />
            &nbsp;<br />
            &nbsp;</p>
    </fieldset>
    <!--fieldset class="ui-corner-all" style="text-align: center;">
        <p>Section 911 mm</p>
        <p>Chord -119 mm</p>
        <p>Value 0.03 mm</p>
    </fieldset -->
    <fieldset class="ui-corner-all" style="text-align: center;">
        <table cellspacing="0" style="width: 100%; border-spacing: 0px; border-collapse: separate;
            text-align: center;">
            <tr>
                <td style="background-color: #8ff; padding: 2px;" id="Scale00">
                    &gt; &gt; 1.10
                </td>
            </tr>
            <tr>
                <td style="background-color: #5f5; padding: 2px;" id="Scale01">
                    0.90 to 1.10
                </td>
            </tr>
            <tr>
                <td style="background-color: #7f7; padding: 2px;" id="Scale02">
                    0.70 to 0.90
                </td>
            </tr>
            <tr>
                <td style="background-color: #9f9; padding: 2px;" id="Scale03">
                    0.50 to 0.70
                </td>
            </tr>
            <tr>
                <td style="background-color: #bfb; padding: 2px;" id="Scale04">
                    0.30 to 0.50
                </td>
            </tr>
            <tr>
                <td style="background-color: #dfd; padding: 2px;" id="Scale05">
                    0.10 to 0.30
                </td>
            </tr>
            <tr>
                <td style="background-color: #fff; padding: 2px;" id="Scale06">
                    -0.10 to 0.10
                </td>
            </tr>
            <tr>
                <td style="background-color: #fdd; padding: 2px;" id="Scale07">
                    -0.10 to -0.30
                </td>
            </tr>
            <tr>
                <td style="background-color: #fbb; padding: 2px;" id="Scale08">
                    -0.30 to -0.50
                </td>
            </tr>
            <tr>
                <td style="background-color: #f99; padding: 2px;" id="Scale09">
                    -0.50 to -0.70
                </td>
            </tr>
            <tr>
                <td style="background-color: #f77; padding: 2px;" id="Scale10">
                    -0.70 to -0.90
                </td>
            </tr>
            <tr>
                <td style="background-color: #f55; padding: 2px;" id="Scale11">
                    -0.90 to -1.10
                </td>
            </tr>
            <tr>
                <td style="background-color: #f8f; padding: 2px;" id="Scale12">
                    &lt; &lt; -1.10
                </td>
            </tr>
        </table>
        <label for="scaleInput">
            Scale</label><input type="text" id="scaleInput" name="scaleInput" size="5" value="0.2" /><br />
        <label for="scaleOffset">
            Offset</label><input type="text" id="scaleOffset" name="scaleOffset" size="5" value="0" /><br />
        <!--input type="checkbox" id="traficLights" name="traficLights" /><label for="traficLights">Traffic
            lights</label><br />
        <input type="checkbox" id="centreLine" name="centreLine" /><label for="centreLine">Centre
            line</label -->
    </fieldset>
</div>
<div style="float: left; width: 80%; height: 95%; min-width: 640px; min-height: 480px;
    padding: 0px;">
    <table class="BladeDisplay">
        <thead>
            <tr>
                <th colspan="2">
                    Outside form
                </th>
                <td>
                    &nbsp;
                </td>
                <th colspan="2">
                    Inside form
                </th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <th rowspan="2">
                    Leading edge
                </th>
                <td rowspan="3" id="ContourPlottingOutsideForm" style="width: 260px; height: 470px;">
                </td>
                <th>
                    ROOT
                </th>
                <td rowspan="3" id="ContourPlottingInsideForm" style="width: 260px; height: 470px;">
                </td>
                <th rowspan="2">
                    Leading edge
                </th>
            </tr>
            <tr>
                <th style="vertical-align: bottom; height: 150px;">
                    Trailing edge
                </th>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <th style="vertical-align: bottom;">
                    TIP
                </th>
                <td>
                    &nbsp;
                </td>
            </tr>
        </tbody>
    </table>
    <script type="text/javascript">
        function RequestImagesOut(strBladeIDs, strCalculate, strCompareTo) {
            $('#ContourPlottingOutsideForm').removeClass('warning');
            $('#ContourPlottingOutsideForm').addClass('loading');
            $('#ContourPlottingOutsideForm').empty();
            var img = new Image();

            img.src = 'API/ImageHandler.ashx?I=ContourBladeImage&BladeIDs=' + strBladeIDs + '&Form=0&Calculate=' + strCalculate + '&CompareTo=' + strCompareTo + '&Scale=' + $('#scaleInput').val() + '&Offset=' + $('#scaleOffset').val();

            $('#ContourPlottingOutsideForm').append(img);
            $(img).fadeIn();
            $('#ContourPlottingOutsideForm').removeClass('loading');
        }
        function RequestImagesIn(strBladeIDs, strCalculate, strCompareTo) {
            var postData = {
                BladeIDs: strBladeIDs,
                Form: "180",
                Calculate: strCalculate,
                CompareTo: strCompareTo,
                Scale: $('#scaleInput').val(),
                Offset: $('#scaleOffset').val()
            };
            $('#ContourPlottingInsideForm').removeClass('warning');
            $('#ContourPlottingInsideForm').addClass('loading');
            $('#ContourPlottingInsideForm').empty();
            var img = new Image();

            img.src = 'API/ImageHandler.ashx?I=ContourBladeImage&BladeIDs=' + strBladeIDs + '&Form=180&Calculate=' + strCalculate + '&CompareTo=' + strCompareTo + '&Scale=' + $('#scaleInput').val() + '&Offset=' + $('#scaleOffset').val();

            $('#ContourPlottingInsideForm').append(img);
            $(img).fadeIn();
            $('#ContourPlottingInsideForm').removeClass('loading');
        }
        function UpdateRequestInfo(Profile, BladeCount, Calculate, CompareTo) {
            $("#RequestInfo").empty();
            var Scale = $('#scaleInput').val();
            var Offset = $('#scaleOffset').val();
            var DisplayText = "<p><strong>" + Profile + "</strong></p>";

            DisplayText += "<p>";
            switch (Calculate) {
                case "0":
                    DisplayText += "Average of<br />" + BladeCount + " Blades<br />from ";

                    switch (CompareTo) {
                        case "0":
                            DisplayText += "Maximum tolerance";
                            break;
                        case "1":
                            DisplayText += "Normal tolerance";
                            break;
                        case "2":
                            DisplayText += "Minimum tolerance";
                            break;
                        case "3":
                            DisplayText += "No tolerances";
                            break;
                    }
                    break;
                case "1": // Standard deviation
                    DisplayText += "Standard deviation of " + BladeCount + " Blades";
                    break;
                case "2": // Spread / max - min
                    DisplayText += "Spread / max - min<br />of " + BladeCount + " Blades";
                    break;
                case "3": // Z usl
                    DisplayText += "Z usl of<br />" + BladeCount + " Blades";
                    break;
                case "4": // Z lsl
                    DisplayText += "Z lsl of<br />" + BladeCount + " Blades";
                    break;
                case "5": // Z min
                    DisplayText += "Z min of<br />" + BladeCount + " Blades";
                    break;
                case "6": // Pp
                    DisplayText += "Pp of<br />" + BladeCount + " Blades";
                    break;
                case "7": // Ppk
                    DisplayText += "Ppk of<br />" + BladeCount + " Blades";
                    break;
            }
            DisplayText += "</p>";

            $("#RequestInfo").html(DisplayText);

            // Scale Key
            var ScaleList = {};
            var top = parseFloat(Offset) + parseFloat((parseFloat(Scale) / 2) + (parseFloat(Scale) * 5));
            for (var i = 0; i < 12; i++) {
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
