// Динамические кнопки имитации обратной связи комплексных стрелок.
partial class MainForm
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        BuildTurnPairFeedbackPanel();
        BuildRouteMatrix();
    }

    private void BuildTurnPairFeedbackPanel()
    {
        _rightButtonPanel.SuspendLayout();

        try
        {
            // Удаляем элементы, созданные Designer: дальше панель строится из Station.
            while (_rightButtonPanel.Controls.Count > 0)
            {
                Control oldControl = _rightButtonPanel.Controls[0];
                _rightButtonPanel.Controls.RemoveAt(0);
                oldControl.Dispose();
            }

            _rightButtonPanel.AutoSize = true;
            _rightButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _rightButtonPanel.Dock = DockStyle.None;
            _rightButtonPanel.FlowDirection = FlowDirection.LeftToRight;
            _rightButtonPanel.WrapContents = false;

            foreach (TurnPair turnPair in sta.turnPairList
                .OrderBy(turn => FirstNumber(turn.name))
                .ThenBy(turn => turn.name, StringComparer.Ordinal))
            {
                _rightButtonPanel.Controls.Add(CreateTurnPairPanel(turnPair));
            }
        }
        finally
        {
            _rightButtonPanel.ResumeLayout(true);
            _buttonPanel.PerformLayout();
        }
    }

    private FlowLayoutPanel CreateTurnPairPanel(TurnPair turnPair)
    {
        FlowLayoutPanel panel = new()
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BorderStyle = BorderStyle.FixedSingle,
            FlowDirection = FlowDirection.TopDown,
            Margin = new Padding(2),
            Padding = new Padding(2),
            WrapContents = false
        };

        panel.Controls.Add(CreateButtonRow(
            CreateFeedbackButton("C+", turnPair.tc.fbC, true),
            CreateFeedbackButton("T+", turnPair.tc.fbT, true)));

        panel.Controls.Add(CreateButtonRow(
            CreateFeedbackButton("C-", turnPair.tc.fbC, false),
            CreateFeedbackButton("T-", turnPair.tc.fbT, false)));

        panel.Controls.Add(new Label
        {
            Font = new Font("Segoe UI", 8F),
            Margin = new Padding(0),
            Size = new Size(60, 18),
            Text = TurnPairCaption(turnPair.name),
            TextAlign = ContentAlignment.MiddleCenter
        });

        return panel;
    }

    private static FlowLayoutPanel CreateButtonRow(params Control[] controls)
    {
        FlowLayoutPanel row = new()
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(1),
            WrapContents = false
        };

        row.Controls.AddRange(controls);
        return row;
    }

    private Button CreateFeedbackButton(string text, Contact contact, bool state)
    {
        Button button = new()
        {
            Font = new Font("Segoe UI", 8F),
            Margin = new Padding(1),
            Size = new Size(28, 20),
            Text = text,
            UseVisualStyleBackColor = true
        };

        button.Click += (_, _) => sta.readTCPContact(contact.addr, contact.input, state);

        return button;
    }

    private static string TurnPairCaption(string name)
    {
        string caption = name.StartsWith("TP", StringComparison.OrdinalIgnoreCase)
            ? name[2..]
            : name;

        return caption.Replace('-', '/');
    }

    private static int FirstNumber(string value)
    {
        int start = 0;
        while (start < value.Length && !char.IsDigit(value[start]))
        {
            start++;
        }

        int end = start;
        while (end < value.Length && char.IsDigit(value[end]))
        {
            end++;
        }

        return start < end && int.TryParse(value[start..end], out int number)
            ? number
            : int.MaxValue;
    }

    // Старые имена обработчиков пока нужны MainForm.cs. Адресов здесь нет:
    // имя стрелки и тип обратной связи определяются по имени элемента.
    private void HandleDesignerTurnPairButton(object? sender)
    {
        if (sender is not Button button
            || !button.Name.StartsWith("button", StringComparison.Ordinal))
        {
            return;
        }

        string[] parts = button.Name["button".Length..].Split('_');
        if (parts.Length < 3)
        {
            return;
        }

        string contactName = parts[^2];
        string stateName = parts[^1];
        if ((contactName != "T" && contactName != "C")
            || (stateName != "On" && stateName != "Off"))
        {
            return;
        }

        string turnPairName = string.Join('-', parts[..^2]);
        SetTurnPairFeedback(
            turnPairName,
            isT: contactName == "T",
            state: stateName == "On");
    }

    private void SetTurnPairFeedback(string name, bool isT, bool state)
    {
        if (sta.atTurnPair(name) is not TurnPair turnPair)
        {
            return;
        }

        Contact contact = isT
            ? turnPair.tc.fbT
            : turnPair.tc.fbC;

        sta.readTCPContact(contact.addr, contact.input, state);
    }

    private void buttonTP1_T_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP1_T_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP1_C_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP1_C_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP2_4_T_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP2_4_T_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP2_4_C_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP2_4_C_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP3_T_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP3_T_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP3_C_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP3_C_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP5_7_T_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP5_7_T_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP5_7_C_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP5_7_C_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP6_8_T_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP6_8_T_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP6_8_C_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP6_8_C_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP9_T_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP9_T_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP9_C_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP9_C_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP10_T_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP10_T_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP10_C_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP10_C_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP12_T_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP12_T_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP12_C_On_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
    private void buttonTP12_C_Off_Click(object? sender, EventArgs e) => HandleDesignerTurnPairButton(sender);
}
