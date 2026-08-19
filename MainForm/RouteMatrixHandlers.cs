// Матрица выбора начальной и конечной кнопок маршрута.
partial class MainForm
{
    private SplitContainer? _routeSplitContainer;
    private DataGridView? _routeMatrix;
    private int _hoveredRouteRow = -1;
    private int _hoveredRouteColumn = -1;
    private static readonly Color RouteHoverColor = Color.FromArgb(255, 210, 96);

    [System.Runtime.CompilerServices.UnsafeAccessor(
        System.Runtime.CompilerServices.UnsafeAccessorKind.Field,
        Name = "startButton")]
    private static extern ref RouteButton? RouteStartButton(RouteControl routeControl);

    [System.Runtime.CompilerServices.UnsafeAccessor(
        System.Runtime.CompilerServices.UnsafeAccessorKind.Field,
        Name = "finishButton")]
    private static extern ref RouteButton? RouteFinishButton(RouteControl routeControl);

    [System.Runtime.CompilerServices.UnsafeAccessor(
        System.Runtime.CompilerServices.UnsafeAccessorKind.Field,
        Name = "direct")]
    private static extern ref Direct RouteDirection(RouteControl routeControl);

    [System.Runtime.CompilerServices.UnsafeAccessor(
        System.Runtime.CompilerServices.UnsafeAccessorKind.Field,
        Name = "typeRoute")]
    private static extern ref TypeRoute RouteType(RouteControl routeControl);

    private void BuildRouteMatrix()
    {
        if (_routeSplitContainer != null)
        {
            return;
        }

        List<RouteButton> routeButtons = sta.routeControl.routeButton
            .OrderBy(button => RouteTypeSortOrder(button.type))
            .ThenBy(button => button.control.addr)
            .ToList();

        _routeMatrix = CreateRouteMatrix(routeButtons);
        _routeSplitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            FixedPanel = FixedPanel.Panel2,
            Orientation = Orientation.Vertical,
            SplitterWidth = 6
        };

        GroupBox matrixGroup = new()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(6),
            Text = "Маршруты: строка — начало, столбец — конец; "
                + "[М] — маневровый, [П] — поездной"
        };
        matrixGroup.Controls.Add(_routeMatrix);

        _contentPanel.SuspendLayout();
        try
        {
            _contentPanel.Controls.Remove(textBox_log);
            _routeSplitContainer.Panel1.Controls.Add(textBox_log);
            _routeSplitContainer.Panel2.Controls.Add(matrixGroup);
            _contentPanel.Controls.Add(_routeSplitContainer, 0, 0);
        }
        finally
        {
            _contentPanel.ResumeLayout(true);
        }

        // Терминал остается видимым слева, матрица получает остальную область.
        _routeSplitContainer.Panel1MinSize = 260;
        _routeSplitContainer.Panel2MinSize = 360;
        _routeSplitContainer.Panel1Collapsed = false;

        int compactMatrixWidth = _routeMatrix.RowHeadersWidth
            + _routeMatrix.Columns
                .Cast<DataGridViewColumn>()
                .Sum(column => column.Width)
            + matrixGroup.Padding.Horizontal
            + 8;
        int maximumMatrixWidth = _routeSplitContainer.ClientSize.Width
            - _routeSplitContainer.Panel1MinSize
            - _routeSplitContainer.SplitterWidth;
        int matrixWidth = Math.Min(compactMatrixWidth, maximumMatrixWidth);

        _routeSplitContainer.SplitterDistance =
            _routeSplitContainer.ClientSize.Width
            - matrixWidth
            - _routeSplitContainer.SplitterWidth;
    }

    private DataGridView CreateRouteMatrix(IReadOnlyList<RouteButton> routeButtons)
    {
        Font headerFont = new("Segoe UI", 9.5F, FontStyle.Bold);
        int longestCaptionWidth = routeButtons
            .Select(button => TextRenderer.MeasureText(
                RouteButtonCaption(button),
                headerFont).Width)
            .DefaultIfEmpty(32)
            .Max();
        int cornerCaptionWidth = TextRenderer.MeasureText("Финиш →", headerFont).Width;

        DataGridView matrix = new()
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToOrderColumns = false,
            AllowUserToResizeRows = false,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            BackgroundColor = SystemColors.Control,
            BorderStyle = BorderStyle.FixedSingle,
            ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable,
            ColumnHeadersHeight = Math.Max(36, longestCaptionWidth + 10),
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            Dock = DockStyle.Fill,
            EditMode = DataGridViewEditMode.EditProgrammatically,
            EnableHeadersVisualStyles = false,
            MultiSelect = false,
            ReadOnly = true,
            RowHeadersWidth = Math.Max(cornerCaptionWidth, longestCaptionWidth) + 8,
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing,
            ScrollBars = ScrollBars.Both,
            SelectionMode = DataGridViewSelectionMode.CellSelect,
            ShowCellToolTips = true
        };

        matrix.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        matrix.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.ControlLight;
        matrix.ColumnHeadersDefaultCellStyle.Font = headerFont;
        matrix.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
        matrix.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        matrix.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
        matrix.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        matrix.RowHeadersDefaultCellStyle.BackColor = SystemColors.ControlLight;
        matrix.RowHeadersDefaultCellStyle.Font = headerFont;
        matrix.RowHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
        matrix.TopLeftHeaderCell.Value = "Старт ↓\nФиниш →";

        foreach (RouteButton finishButton in routeButtons)
        {
            DataGridViewButtonColumn column = new()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                FlatStyle = FlatStyle.Flat,
                HeaderText = RouteButtonCaption(finishButton),
                MinimumWidth = 20,
                Name = $"routeFinish_{finishButton.control.addr}",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Tag = finishButton,
                Text = "▶",
                UseColumnTextForButtonValue = true,
                Width = 22
            };

            column.HeaderCell.Style.BackColor = RouteTypeHeaderColor(finishButton.type);
            column.HeaderCell.Style.ForeColor = Color.Black;
            matrix.Columns.Add(column);
        }

        foreach (RouteButton startButton in routeButtons)
        {
            int rowIndex = matrix.Rows.Add();
            DataGridViewRow row = matrix.Rows[rowIndex];
            row.HeaderCell.Value = RouteButtonCaption(startButton);
            row.HeaderCell.Style.BackColor = RouteTypeHeaderColor(startButton.type);
            row.HeaderCell.Style.ForeColor = Color.Black;
            row.Height = 20;
            row.Tag = startButton;

            foreach (DataGridViewColumn column in matrix.Columns)
            {
                RouteButton finishButton = (RouteButton)column.Tag!;
                DataGridViewCell cell = row.Cells[column.Index];
                List<Route>? path = FindPathForMatrix(startButton, finishButton);

                if (path == null)
                {
                    cell.ToolTipText = startButton == finishButton
                        ? "Начальная и конечная кнопки совпадают"
                        : startButton.type != finishButton.type
                            ? "Типы начальной и конечной кнопок не совпадают"
                            : "Путь между кнопками не найден";
                    cell.Style.BackColor = Color.FromArgb(255, 202, 202);
                    cell.Style.ForeColor = Color.FromArgb(130, 0, 0);
                    cell.Style.SelectionBackColor = Color.FromArgb(238, 118, 118);
                    cell.Style.SelectionForeColor = Color.Black;
                }
                else
                {
                    cell.ToolTipText = $"{RouteButtonDescription(startButton)} → "
                        + $"{RouteButtonDescription(finishButton)}\n"
                        + $"Путь: {string.Join(" → ", path.Select(route => route.name))}";
                    cell.Style.BackColor = RouteTypeCellColor(startButton.type);
                    cell.Style.ForeColor = Color.Black;
                    cell.Style.SelectionBackColor = RouteTypeSelectionColor(startButton.type);
                    cell.Style.SelectionForeColor = Color.Black;
                }
            }
        }

        matrix.CellContentClick += RouteMatrix_CellContentClick;
        matrix.CellMouseEnter += RouteMatrix_CellMouseEnter;
        matrix.CellPainting += RouteMatrix_CellPainting;
        matrix.MouseLeave += RouteMatrix_MouseLeave;
        matrix.ClearSelection();
        return matrix;
    }

    private void RouteMatrix_CellMouseEnter(object? sender, DataGridViewCellEventArgs e)
    {
        if (sender is DataGridView matrix)
        {
            SetRouteMatrixHover(matrix, e.RowIndex, e.ColumnIndex);
        }
    }

    private void RouteMatrix_MouseLeave(object? sender, EventArgs e)
    {
        if (sender is DataGridView matrix)
        {
            SetRouteMatrixHover(matrix, -1, -1);
        }
    }

    private void SetRouteMatrixHover(DataGridView matrix, int rowIndex, int columnIndex)
    {
        if (_hoveredRouteRow == rowIndex && _hoveredRouteColumn == columnIndex)
        {
            return;
        }

        int previousRow = _hoveredRouteRow;
        int previousColumn = _hoveredRouteColumn;
        _hoveredRouteRow = rowIndex;
        _hoveredRouteColumn = columnIndex;

        if (previousRow != _hoveredRouteRow)
        {
            InvalidateRouteRowHeader(matrix, previousRow);
            InvalidateRouteRowHeader(matrix, _hoveredRouteRow);
        }

        if (previousColumn != _hoveredRouteColumn)
        {
            InvalidateRouteColumnHeader(matrix, previousColumn);
            InvalidateRouteColumnHeader(matrix, _hoveredRouteColumn);
        }
    }

    private static void InvalidateRouteRowHeader(DataGridView matrix, int rowIndex)
    {
        if (rowIndex >= 0 && rowIndex < matrix.Rows.Count)
        {
            matrix.InvalidateCell(matrix.Rows[rowIndex].HeaderCell);
        }
    }

    private static void InvalidateRouteColumnHeader(DataGridView matrix, int columnIndex)
    {
        if (columnIndex >= 0 && columnIndex < matrix.Columns.Count)
        {
            matrix.InvalidateCell(matrix.Columns[columnIndex].HeaderCell);
        }
    }

    private void RouteMatrix_CellPainting(
        object? sender,
        DataGridViewCellPaintingEventArgs e)
    {
        if (sender is not DataGridView matrix
            || e.Graphics is not Graphics graphics)
        {
            return;
        }

        // Рисуем подписи строк сами, без зарезервированного места под указатель строки.
        if (e.ColumnIndex == -1 && e.RowIndex >= 0)
        {
            PaintRouteHeaderBackground(
                graphics,
                e,
                e.RowIndex == _hoveredRouteRow);

            string rowText = matrix.Rows[e.RowIndex].HeaderCell.Value?.ToString() ?? "";
            DataGridViewCellStyle rowStyle = e.CellStyle
                ?? matrix.RowHeadersDefaultCellStyle;
            Font rowFont = rowStyle.Font ?? matrix.Font;
            Color rowColor = rowStyle.ForeColor.IsEmpty
                ? matrix.ForeColor
                : rowStyle.ForeColor;

            TextRenderer.DrawText(
                graphics,
                rowText,
                rowFont,
                e.CellBounds,
                rowColor,
                TextFormatFlags.HorizontalCenter
                    | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.SingleLine
                    | TextFormatFlags.NoPadding);

            e.Handled = true;
            return;
        }

        if (e.RowIndex != -1 || e.ColumnIndex < 0)
        {
            return;
        }

        PaintRouteHeaderBackground(
            graphics,
            e,
            e.ColumnIndex == _hoveredRouteColumn);

        string text = matrix.Columns[e.ColumnIndex].HeaderText;
        DataGridViewCellStyle cellStyle = e.CellStyle
            ?? matrix.ColumnHeadersDefaultCellStyle;
        Font font = cellStyle.Font ?? matrix.Font;
        Color color = cellStyle.ForeColor.IsEmpty
            ? matrix.ForeColor
            : cellStyle.ForeColor;

        using Brush textBrush = new SolidBrush(color);
        var graphicsState = graphics.Save();

        try
        {
            // Начинаем снизу и поворачиваем подпись на 90 градусов против часовой стрелки.
            graphics.TranslateTransform(
                e.CellBounds.Left + e.CellBounds.Width / 2F,
                e.CellBounds.Bottom - 5F);
            graphics.RotateTransform(-90F);

            SizeF textSize = graphics.MeasureString(text, font);
            graphics.DrawString(
                text,
                font,
                textBrush,
                0F,
                -textSize.Height / 2F);
        }
        finally
        {
            graphics.Restore(graphicsState);
        }

        e.Handled = true;
    }

    private static void PaintRouteHeaderBackground(
        Graphics graphics,
        DataGridViewCellPaintingEventArgs e,
        bool highlighted)
    {
        if (highlighted)
        {
            using Brush hoverBrush = new SolidBrush(RouteHoverColor);
            graphics.FillRectangle(hoverBrush, e.CellBounds);
            e.Paint(e.CellBounds, DataGridViewPaintParts.Border);
            return;
        }

        e.Paint(
            e.ClipBounds,
            DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);
    }

    private void RouteMatrix_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (sender is not DataGridView matrix
            || e.RowIndex < 0
            || e.ColumnIndex < 0
            || matrix.Rows[e.RowIndex].Cells[e.ColumnIndex] is not DataGridViewButtonCell
            || matrix.Rows[e.RowIndex].Tag is not RouteButton startButton
            || matrix.Columns[e.ColumnIndex].Tag is not RouteButton finishButton)
        {
            return;
        }

        try
        {
            // sta.clearAllRoutes();
            // sta.clearAllTrafficLights();
            sta.routeControl.pressRouteButton(startButton.control);
            sta.routeControl.pressRouteButton(finishButton.control);
        }
        catch (Exception ex)
        {
            ConsoleLog.print(
                $"Ошибка построения маршрута {RouteButtonDescription(startButton)} -> "
                + $"{RouteButtonDescription(finishButton)}: {ex.Message}",
                Color.Red);
        }
        finally
        {
            matrix.ClearSelection();
        }
    }

    private List<Route>? FindPathForMatrix(RouteButton startButton, RouteButton finishButton)
    {
        if (startButton == finishButton || startButton.type != finishButton.type)
        {
            return null;
        }

        TrafficLight? trafficLight = startButton.bridge.trafficLight;
        if (trafficLight == null)
        {
            return null;
        }

        RouteControl routeControl = sta.routeControl;
        ref RouteButton? currentStartButton = ref RouteStartButton(routeControl);
        ref RouteButton? currentFinishButton = ref RouteFinishButton(routeControl);
        ref Direct currentDirection = ref RouteDirection(routeControl);
        ref TypeRoute currentType = ref RouteType(routeControl);

        RouteButton? savedStartButton = currentStartButton;
        RouteButton? savedFinishButton = currentFinishButton;
        Direct savedDirection = currentDirection;
        TypeRoute savedType = currentType;

        try
        {
            currentStartButton = startButton;
            currentFinishButton = finishButton;
            currentDirection = trafficLight.dir;
            currentType = startButton.type == RouteButton.Type.Shunt
                ? TypeRoute.Shunt
                : TypeRoute.Train;

            return routeControl.findPath(startButton.bridge, finishButton.bridge);
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            currentStartButton = savedStartButton;
            currentFinishButton = savedFinishButton;
            currentDirection = savedDirection;
            currentType = savedType;
        }
    }

    private static string RouteButtonCaption(RouteButton button)
    {
        string typeCaption = button.type switch
        {
            RouteButton.Type.Train => "П",
            RouteButton.Type.Shunt => "М",
            RouteButton.Type.End => "К",
            _ => "?"
        };

        return $"{button.name} [{typeCaption}]";
    }

    private static string RouteButtonDescription(RouteButton button)
    {
        string typeDescription = button.type switch
        {
            RouteButton.Type.Train => "поездной",
            RouteButton.Type.Shunt => "маневровый",
            RouteButton.Type.End => "конечный",
            _ => "неизвестный"
        };

        return $"{button.name} ({typeDescription})";
    }

    private static Color RouteTypeHeaderColor(RouteButton.Type type) => type switch
    {
        RouteButton.Type.Shunt => Color.FromArgb(190, 220, 255),
        RouteButton.Type.Train => Color.FromArgb(195, 232, 199),
        RouteButton.Type.End => Color.FromArgb(255, 229, 166),
        _ => SystemColors.ControlLight
    };

    private static Color RouteTypeCellColor(RouteButton.Type type) => type switch
    {
        RouteButton.Type.Shunt => Color.FromArgb(232, 243, 255),
        RouteButton.Type.Train => Color.FromArgb(234, 247, 235),
        RouteButton.Type.End => Color.FromArgb(255, 246, 221),
        _ => SystemColors.Window
    };

    private static Color RouteTypeSelectionColor(RouteButton.Type type) => type switch
    {
        RouteButton.Type.Shunt => Color.FromArgb(135, 184, 240),
        RouteButton.Type.Train => Color.FromArgb(143, 207, 151),
        RouteButton.Type.End => Color.FromArgb(238, 196, 95),
        _ => SystemColors.Highlight
    };

    private static int RouteTypeSortOrder(RouteButton.Type type) => type switch
    {
        RouteButton.Type.Shunt => 0,
        RouteButton.Type.Train => 1,
        RouteButton.Type.End => 2,
        _ => int.MaxValue
    };
}
