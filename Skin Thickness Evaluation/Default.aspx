<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Skin_Thickness_Evaluation._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#SubBladesSelect').hide();

            $('input:radio').click(function () {
                if ($('#SubBlades').is(':checked')) {
                    $('#SubBladesSelect').show('slow');
                } else {
                    $('#SubBladesSelect').hide('slow');
                }
            });

            $.ajax({
                type: "GET",
                url: "Data/SELETAR-FBSG.xml",
                dataType: "xml",
                success: function (xml) {
                    //var select = $('#ddlStationName');

                    $(xml).find('EQUIPMENT[CLASS="PLT-BASE.RS"]').each(function () {
                        var title = $(this).attr('ID');

                        if (title != "") {
                            //select.append("<option>" + title + "</option>");

                            $("<option>").attr("value", title).text(title).appendTo("#ddlStationName");
                        }
                    });
                }
            });

            var dates = $("#DateStart, #DateStop").datepicker({
                showOn: "button",
                buttonImage: "Images/calendar.png",
                buttonImageOnly: true,
                defaultDate: "-2w",
                changeMonth: true,
                onSelect: function (selectedDate) {
                    var option = this.id == "DateStart" ? "minDate" : "maxDate",
					instance = $(this).data("datepicker"),
					date = $.datepicker.parseDate(
						instance.settings.dateFormat ||
						$.datepicker._defaults.dateFormat,
						selectedDate, instance.settings);
                    dates.not(this).datepicker("option", option, date);
                }
            });

            $("#ddlStationName").combobox();
        });

        (function ($) {
            $.widget("ui.combobox", {
                _create: function () {
                    var self = this,
					select = this.element.hide(),
					selected = select.children(":selected"),
					value = selected.val() ? selected.text() : "";
                    var input = this.input = $("<input>")
					.insertAfter(select)
					.val(value)
					.autocomplete({
					    delay: 0,
					    minLength: 0,
					    source: function (request, response) {
					        var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
					        response(select.children("option").map(function () {
					            var text = $(this).text();
					            if (this.value && (!request.term || matcher.test(text)))
					                return {
					                    label: text.replace(
											new RegExp(
												"(?![^&;]+;)(?!<[^<>]*)(" +
												$.ui.autocomplete.escapeRegex(request.term) +
												")(?![^<>]*>)(?![^&;]+;)", "gi"
											), "<strong>$1</strong>"),
					                    value: text,
					                    option: this
					                };
					        }));
					    },
					    select: function (event, ui) {
					        ui.item.option.selected = true;
					        self._trigger("selected", event, {
					            item: ui.item.option
					        });
					    },
					    change: function (event, ui) {
					        if (!ui.item) {
					            var matcher = new RegExp("^" + $.ui.autocomplete.escapeRegex($(this).val()) + "$", "i"),
									valid = false;
					            select.children("option").each(function () {
					                if ($(this).text().match(matcher)) {
					                    this.selected = valid = true;
					                    return false;
					                }
					            });
					            if (!valid) {
					                // remove invalid value, as it didn't match anything
					                $(this).val("");
					                select.val("");
					                input.data("autocomplete").term = "";
					                return false;
					            }
					        }
					    }
					})
					.addClass("ui-widget ui-widget-content ui-corner-left");

                    input.data("autocomplete")._renderItem = function (ul, item) {
                        return $("<li></li>")
						.data("item.autocomplete", item)
						.append("<a>" + item.label + "</a>")
						.appendTo(ul);
                    };

                    this.button = $("<button type='button'>&nbsp;</button>")
					.attr("tabIndex", -1)
					.attr("title", "Show All Items")
					.insertAfter(input)
					.button({
					    icons: {
					        primary: "ui-icon-triangle-1-s"
					    },
					    text: false
					})
					.removeClass("ui-corner-all")
					.addClass("ui-corner-right ui-button-icon")
					.click(function () {
					    // close if already visible
					    if (input.autocomplete("widget").is(":visible")) {
					        input.autocomplete("close");
					        return;
					    }

					    // work around a bug (likely same cause as #5265)
					    $(this).blur();

					    // pass empty string as value to search for, displaying all results
					    input.autocomplete("search", "");
					    input.focus();
					});
                },

                destroy: function () {
                    this.input.remove();
                    this.button.remove();
                    this.element.show();
                    $.Widget.prototype.destroy.call(this);
                }
            });
        })(jQuery);
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <input type="hidden" id="UseCase" name="UseCase" value="" />
    <h2>
        Navigation of Skin Thickness Evaluation</h2>
    <ul style="width: 49%; float: right; list-style-type: none;">
        <li><a href="#" onclick="NavigateTo('UC004');return false;" class="button">Contour Plotting
            Filtering and calculation</a></li>
        <li><a href="#" onclick="NavigateTo('UC005');return false;" class="button">SPC Analysis
            Filtering</a></li>
        <li><a href="#" onclick="NavigateTo('UC006');return false;" class="button">Export Analysis
            Filtering</a></li>
        <li><a href="#" onclick="NavigateTo('UC007');return false;" class="button">Rework Analysis
            Filtering</a></li>
        <li><a href="#" onclick="NavigateTo('UC008');return false;" class="button">Metal Removal
            Filtering</a></li>
        <li><a href="#" onclick="NavigateTo('UC009');return false;" class="button">Cavity Reports
            Analysis Filtering</a></li>
    </ul>
    <div style="width: 49%; display: inline-block;">
        <div style="display: block;">
            <input id="AllBlades" type="radio" name="DataSet" checked="checked" />
            <label for="AllBlades">
                All Blades</label>
            <input id="SubBlades" type="radio" name="DataSet" />
            <label for="SubBlades">
                Resource Blade Set</label>
        </div>
        <div id="SubBladesSelect">
            <div style="clear: both;">
                <label for="ddlStationName">
                    Station Name</label>
                <select id="ddlStationName" style="width: 100%">
                </select>
            </div>
            <div style="clear: both; display: inline-block;">
                <div style="clear: both;">
                    Between these dates</div>
                <div style="float: right; width: 100%; text-align: right;">
                    <label for="DateStart">
                        From</label>
                    <input type="text" name="DateStart" id="DateStart" class="datepicker" />
                </div>
                <div style="float: right; width: 100%; text-align: right;">
                    <label for="DateStop">
                        To</label>
                    <input type="text" name="DateStop" id="DateStop" class="datepicker" />
                </div>
            </div>
            <div style="clear: both; text-align: right;">
                <a href="#" class="button" onclick="return GetResourceBladeSet();">Update List</a>
            </div>
            <div style="clear: both;">
                Blade List<br />
                <select id="mslBladeList" name="mslBladeList" multiple="multiple" size="10" style="width: 100%">
                </select>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function GetResourceBladeSet() {
            var postData = {
                resourceName: $('#ddlStationName option:selected').text(),
                dateStart: $("#DateStart").val(),
                dateEnd: $("#DateStop").val()
            }

            $.ajax({
                type: 'POST',
                url: 'API/AJAX_Requests.asmx/Select_Resource_BladeSet',
                data: JSON.stringify(postData),
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                success: function (msg) {
                    $('#mslBladeList').empty();
                    var c = eval(msg.d);
                    for (var i in c) {
                        //$('#mslBladeList').append(new Option(c[i][1], c[i][1]));
                        $("<option>").attr("value", c[i][1]).text(c[i][1]).appendTo("#mslBladeList");

                    }
                    $('#mslBladeList option').each(function () {
                        $(this).attr('selected', 'selected');
                    })
                }
            });
            return false;
        }

        function NavigateTo(pageID) {
            var postData;

            $('#UseCase').val(pageID);

            if ($('#SubBlades').is(':checked')) {
                if ($('#mslBladeList').val() != null) {
                    postData = { BladeList: ($('#mslBladeList').val()).join(',') }
                } else {
                    alert("No Blades Selected");
                    return;
                }
            } else {
                postData = { BladeList: '' }
            }

            $.ajax({
                type: 'POST',
                url: 'API/AJAX_Requests.asmx/Set_Resource_BladeSet',
                data: JSON.stringify(postData),
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                success: function (msg) {
                    $("form").submit(); // = pageID+'.aspx';
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="FootContent" runat="server" ContentPlaceHolderID="FooterContent">
</asp:Content>
