#nullable enable

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer? components = null;

#nullable disable

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            server.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        textBox_log = new RichTextBox();
        textBox_cmd = new TextBox();
        sendButton = new Button();
        helpButton = new Button();
        clearButton = new Button();
        serverCheckBox = new CheckBox();
        debugCheckBox = new CheckBox();
        _buttonPanel = new FlowLayoutPanel();
        button_powerOn = new Button();
        button_powerOff = new Button();
        _rightButtonPanel = new FlowLayoutPanel();
        flowLayoutPanel1 = new FlowLayoutPanel();
        flowLayoutPanel2 = new FlowLayoutPanel();
        flowLayoutPanel3 = new FlowLayoutPanel();
        flowLayoutPanel4 = new FlowLayoutPanel();
        buttonTP1_T_On = new Button();
        buttonTP1_C_On = new Button();
        flowLayoutPanel5 = new FlowLayoutPanel();
        buttonTP1_T_Off = new Button();
        buttonTP1_C_Off = new Button();
        labelTP1 = new Label();
        flowLayoutPanel6 = new FlowLayoutPanel();
        flowLayoutPanel7 = new FlowLayoutPanel();
        buttonTP2_4_T_On = new Button();
        buttonTP2_4_C_On = new Button();
        flowLayoutPanel8 = new FlowLayoutPanel();
        buttonTP2_4_T_Off = new Button();
        buttonTP2_4_C_Off = new Button();
        labelTP2_4 = new Label();
        flowLayoutPanel9 = new FlowLayoutPanel();
        flowLayoutPanel10 = new FlowLayoutPanel();
        buttonTP3_T_On = new Button();
        buttonTP3_C_On = new Button();
        flowLayoutPanel11 = new FlowLayoutPanel();
        buttonTP3_T_Off = new Button();
        buttonTP3_C_Off = new Button();
        labelTP3 = new Label();
        flowLayoutPanel12 = new FlowLayoutPanel();
        flowLayoutPanel13 = new FlowLayoutPanel();
        buttonTP5_7_T_On = new Button();
        buttonTP5_7_C_On = new Button();
        flowLayoutPanel14 = new FlowLayoutPanel();
        buttonTP5_7_T_Off = new Button();
        buttonTP5_7_C_Off = new Button();
        labelTP5_7 = new Label();
        flowLayoutPanel15 = new FlowLayoutPanel();
        flowLayoutPanel16 = new FlowLayoutPanel();
        buttonTP6_8_T_On = new Button();
        buttonTP6_8_C_On = new Button();
        flowLayoutPanel17 = new FlowLayoutPanel();
        buttonTP6_8_T_Off = new Button();
        buttonTP6_8_C_Off = new Button();
        labelTP6_8 = new Label();
        flowLayoutPanel18 = new FlowLayoutPanel();
        flowLayoutPanel19 = new FlowLayoutPanel();
        buttonTP9_T_On = new Button();
        buttonTP9_C_On = new Button();
        flowLayoutPanel20 = new FlowLayoutPanel();
        buttonTP9_T_Off = new Button();
        buttonTP9_C_Off = new Button();
        labelTP9 = new Label();
        flowLayoutPanel21 = new FlowLayoutPanel();
        flowLayoutPanel22 = new FlowLayoutPanel();
        buttonTP10_T_On = new Button();
        buttonTP10_C_On = new Button();
        flowLayoutPanel23 = new FlowLayoutPanel();
        buttonTP10_T_Off = new Button();
        buttonTP10_C_Off = new Button();
        labelTP10 = new Label();
        flowLayoutPanel24 = new FlowLayoutPanel();
        flowLayoutPanel25 = new FlowLayoutPanel();
        buttonTP12_T_On = new Button();
        buttonTP12_C_On = new Button();
        flowLayoutPanel26 = new FlowLayoutPanel();
        buttonTP12_T_Off = new Button();
        buttonTP12_C_Off = new Button();
        labelTP12 = new Label();
        _contentPanel = new TableLayoutPanel();
        _commandPanel = new TableLayoutPanel();
        _buttonPanel.SuspendLayout();
        _rightButtonPanel.SuspendLayout();
        flowLayoutPanel3.SuspendLayout();
        flowLayoutPanel4.SuspendLayout();
        flowLayoutPanel5.SuspendLayout();
        flowLayoutPanel6.SuspendLayout();
        flowLayoutPanel7.SuspendLayout();
        flowLayoutPanel8.SuspendLayout();
        flowLayoutPanel9.SuspendLayout();
        flowLayoutPanel10.SuspendLayout();
        flowLayoutPanel11.SuspendLayout();
        flowLayoutPanel12.SuspendLayout();
        flowLayoutPanel13.SuspendLayout();
        flowLayoutPanel14.SuspendLayout();
        flowLayoutPanel15.SuspendLayout();
        flowLayoutPanel16.SuspendLayout();
        flowLayoutPanel17.SuspendLayout();
        flowLayoutPanel18.SuspendLayout();
        flowLayoutPanel19.SuspendLayout();
        flowLayoutPanel20.SuspendLayout();
        flowLayoutPanel21.SuspendLayout();
        flowLayoutPanel22.SuspendLayout();
        flowLayoutPanel23.SuspendLayout();
        flowLayoutPanel24.SuspendLayout();
        flowLayoutPanel25.SuspendLayout();
        flowLayoutPanel26.SuspendLayout();
        _contentPanel.SuspendLayout();
        _commandPanel.SuspendLayout();
        SuspendLayout();
        // 
        // textBox_log
        // 
        textBox_log.BackColor = Color.FromArgb(30, 30, 30);
        textBox_log.DetectUrls = false;
        textBox_log.Dock = DockStyle.Fill;
        textBox_log.Font = new Font("Consolas", 10F);
        textBox_log.ForeColor = Color.Gainsboro;
        textBox_log.Location = new Point(3, 3);
        textBox_log.Name = "textBox_log";
        textBox_log.ReadOnly = true;
        textBox_log.Size = new Size(1373, 632);
        textBox_log.TabIndex = 1;
        textBox_log.Text = "";
        textBox_log.WordWrap = false;
        // 
        // textBox_cmd
        // 
        textBox_cmd.Dock = DockStyle.Fill;
        textBox_cmd.Font = new Font("Consolas", 10F);
        textBox_cmd.Location = new Point(11, 11);
        textBox_cmd.Name = "textBox_cmd";
        textBox_cmd.Size = new Size(1290, 23);
        textBox_cmd.TabIndex = 0;
        // 
        // sendButton
        // 
        sendButton.AutoSize = true;
        sendButton.Location = new Point(1307, 11);
        sendButton.Name = "sendButton";
        sendButton.Size = new Size(61, 25);
        sendButton.TabIndex = 1;
        sendButton.Text = "Send";
        sendButton.UseVisualStyleBackColor = true;
        // 
        // helpButton
        // 
        helpButton.AutoSize = true;
        helpButton.Location = new Point(11, 11);
        helpButton.Name = "helpButton";
        helpButton.Size = new Size(48, 25);
        helpButton.TabIndex = 0;
        helpButton.Text = "Help";
        helpButton.UseVisualStyleBackColor = true;
        // 
        // clearButton
        // 
        clearButton.AutoSize = true;
        clearButton.Location = new Point(196, 11);
        clearButton.Name = "clearButton";
        clearButton.Size = new Size(67, 25);
        clearButton.TabIndex = 2;
        clearButton.Text = "Clear log";
        clearButton.UseVisualStyleBackColor = true;
        // 
        // serverCheckBox
        // 
        serverCheckBox.AutoSize = true;
        serverCheckBox.Location = new Point(65, 14);
        serverCheckBox.Margin = new Padding(3, 6, 3, 0);
        serverCheckBox.Name = "serverCheckBox";
        serverCheckBox.Size = new Size(58, 19);
        serverCheckBox.TabIndex = 1;
        serverCheckBox.Text = "Server";
        serverCheckBox.UseVisualStyleBackColor = true;
        // 
        // debugCheckBox
        // 
        debugCheckBox.AutoSize = true;
        debugCheckBox.Location = new Point(129, 14);
        debugCheckBox.Margin = new Padding(3, 6, 3, 0);
        debugCheckBox.Name = "debugCheckBox";
        debugCheckBox.Size = new Size(61, 19);
        debugCheckBox.TabIndex = 2;
        debugCheckBox.Text = "Debug";
        debugCheckBox.UseVisualStyleBackColor = true;
        // 
        // _buttonPanel
        // 
        _buttonPanel.AutoSize = true;
        _buttonPanel.Controls.Add(helpButton);
        _buttonPanel.Controls.Add(serverCheckBox);
        _buttonPanel.Controls.Add(debugCheckBox);
        _buttonPanel.Controls.Add(clearButton);
        _buttonPanel.Controls.Add(button_powerOn);
        _buttonPanel.Controls.Add(button_powerOff);
        _buttonPanel.Controls.Add(_rightButtonPanel);
        _buttonPanel.Dock = DockStyle.Top;
        _buttonPanel.Location = new Point(0, 0);
        _buttonPanel.Name = "_buttonPanel";
        _buttonPanel.Padding = new Padding(8);
        _buttonPanel.Size = new Size(1379, 110);
        _buttonPanel.TabIndex = 0;
        // 
        // button_powerOn
        // 
        button_powerOn.Location = new Point(269, 11);
        button_powerOn.Name = "button_powerOn";
        button_powerOn.Size = new Size(75, 23);
        button_powerOn.TabIndex = 3;
        button_powerOn.Text = "Power ON";
        button_powerOn.UseVisualStyleBackColor = true;
        button_powerOn.Click += button_powerOn_Click_1;
        // 
        // button_powerOff
        // 
        button_powerOff.Location = new Point(350, 11);
        button_powerOff.Name = "button_powerOff";
        button_powerOff.Size = new Size(79, 23);
        button_powerOff.TabIndex = 4;
        button_powerOff.Text = "Power OFF";
        button_powerOff.UseVisualStyleBackColor = true;
        // 
        // _rightButtonPanel
        // 
        _rightButtonPanel.AutoSize = true;
        _rightButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _rightButtonPanel.BackColor = SystemColors.Control;
        _rightButtonPanel.BorderStyle = BorderStyle.FixedSingle;
        _rightButtonPanel.Controls.Add(flowLayoutPanel1);
        _rightButtonPanel.Controls.Add(flowLayoutPanel2);
        _rightButtonPanel.Controls.Add(flowLayoutPanel3);
        _rightButtonPanel.Controls.Add(flowLayoutPanel6);
        _rightButtonPanel.Controls.Add(flowLayoutPanel9);
        _rightButtonPanel.Controls.Add(flowLayoutPanel12);
        _rightButtonPanel.Controls.Add(flowLayoutPanel15);
        _rightButtonPanel.Controls.Add(flowLayoutPanel18);
        _rightButtonPanel.Controls.Add(flowLayoutPanel21);
        _rightButtonPanel.Controls.Add(flowLayoutPanel24);
        _rightButtonPanel.Dock = DockStyle.Fill;
        _rightButtonPanel.Location = new Point(432, 8);
        _rightButtonPanel.Margin = new Padding(0);
        _rightButtonPanel.Name = "_rightButtonPanel";
        _rightButtonPanel.Padding = new Padding(8);
        _rightButtonPanel.Size = new Size(606, 94);
        _rightButtonPanel.TabIndex = 1;
        // 
        // flowLayoutPanel1
        // 
        flowLayoutPanel1.AutoSize = true;
        flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
        flowLayoutPanel1.Location = new Point(11, 11);
        flowLayoutPanel1.Name = "flowLayoutPanel1";
        flowLayoutPanel1.Size = new Size(0, 0);
        flowLayoutPanel1.TabIndex = 2;
        // 
        // flowLayoutPanel2
        // 
        flowLayoutPanel2.AutoSize = true;
        flowLayoutPanel2.FlowDirection = FlowDirection.TopDown;
        flowLayoutPanel2.Location = new Point(17, 11);
        flowLayoutPanel2.Name = "flowLayoutPanel2";
        flowLayoutPanel2.Size = new Size(0, 0);
        flowLayoutPanel2.TabIndex = 3;
        // 
        // flowLayoutPanel3
        // 
        flowLayoutPanel3.AutoSize = true;
        flowLayoutPanel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel3.BorderStyle = BorderStyle.FixedSingle;
        flowLayoutPanel3.Controls.Add(flowLayoutPanel4);
        flowLayoutPanel3.Controls.Add(flowLayoutPanel5);
        flowLayoutPanel3.Controls.Add(labelTP1);
        flowLayoutPanel3.FlowDirection = FlowDirection.TopDown;
        flowLayoutPanel3.Location = new Point(22, 10);
        flowLayoutPanel3.Margin = new Padding(2);
        flowLayoutPanel3.Name = "flowLayoutPanel3";
        flowLayoutPanel3.Padding = new Padding(2);
        flowLayoutPanel3.Size = new Size(68, 72);
        flowLayoutPanel3.TabIndex = 4;
        flowLayoutPanel3.WrapContents = false;
        // 
        // flowLayoutPanel4
        // 
        flowLayoutPanel4.AutoSize = true;
        flowLayoutPanel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel4.Controls.Add(buttonTP1_T_On);
        flowLayoutPanel4.Controls.Add(buttonTP1_C_On);
        flowLayoutPanel4.Location = new Point(3, 3);
        flowLayoutPanel4.Margin = new Padding(1);
        flowLayoutPanel4.Name = "flowLayoutPanel4";
        flowLayoutPanel4.Size = new Size(60, 22);
        flowLayoutPanel4.TabIndex = 0;
        flowLayoutPanel4.WrapContents = false;
        // 
        // buttonTP1_T_On
        // 
        buttonTP1_T_On.Font = new Font("Segoe UI", 8F);
        buttonTP1_T_On.Location = new Point(1, 1);
        buttonTP1_T_On.Margin = new Padding(1);
        buttonTP1_T_On.Name = "buttonTP1_T_On";
        buttonTP1_T_On.Size = new Size(28, 20);
        buttonTP1_T_On.TabIndex = 1;
        buttonTP1_T_On.Text = "T+";
        buttonTP1_T_On.UseVisualStyleBackColor = true;
        // 
        // buttonTP1_C_On
        // 
        buttonTP1_C_On.Font = new Font("Segoe UI", 8F);
        buttonTP1_C_On.Location = new Point(31, 1);
        buttonTP1_C_On.Margin = new Padding(1);
        buttonTP1_C_On.Name = "buttonTP1_C_On";
        buttonTP1_C_On.Size = new Size(28, 20);
        buttonTP1_C_On.TabIndex = 1;
        buttonTP1_C_On.Text = "C+";
        buttonTP1_C_On.UseVisualStyleBackColor = true;
        // 
        // flowLayoutPanel5
        // 
        flowLayoutPanel5.AutoSize = true;
        flowLayoutPanel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel5.Controls.Add(buttonTP1_T_Off);
        flowLayoutPanel5.Controls.Add(buttonTP1_C_Off);
        flowLayoutPanel5.Location = new Point(3, 27);
        flowLayoutPanel5.Margin = new Padding(1);
        flowLayoutPanel5.Name = "flowLayoutPanel5";
        flowLayoutPanel5.Size = new Size(60, 22);
        flowLayoutPanel5.TabIndex = 1;
        flowLayoutPanel5.WrapContents = false;
        // 
        // buttonTP1_T_Off
        // 
        buttonTP1_T_Off.Font = new Font("Segoe UI", 8F);
        buttonTP1_T_Off.Location = new Point(1, 1);
        buttonTP1_T_Off.Margin = new Padding(1);
        buttonTP1_T_Off.Name = "buttonTP1_T_Off";
        buttonTP1_T_Off.Size = new Size(28, 20);
        buttonTP1_T_Off.TabIndex = 0;
        buttonTP1_T_Off.Text = "T-";
        buttonTP1_T_Off.UseVisualStyleBackColor = true;
        // 
        // buttonTP1_C_Off
        // 
        buttonTP1_C_Off.Font = new Font("Segoe UI", 8F);
        buttonTP1_C_Off.Location = new Point(31, 1);
        buttonTP1_C_Off.Margin = new Padding(1);
        buttonTP1_C_Off.Name = "buttonTP1_C_Off";
        buttonTP1_C_Off.Size = new Size(28, 20);
        buttonTP1_C_Off.TabIndex = 0;
        buttonTP1_C_Off.Text = "C-";
        buttonTP1_C_Off.UseVisualStyleBackColor = true;
        // 
        // labelTP1
        // 
        labelTP1.Font = new Font("Segoe UI", 8F);
        labelTP1.Location = new Point(2, 50);
        labelTP1.Margin = new Padding(0);
        labelTP1.Name = "labelTP1";
        labelTP1.Size = new Size(60, 18);
        labelTP1.TabIndex = 0;
        labelTP1.Text = "1";
        labelTP1.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // flowLayoutPanel6
        // 
        flowLayoutPanel6.AutoSize = true;
        flowLayoutPanel6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel6.BorderStyle = BorderStyle.FixedSingle;
        flowLayoutPanel6.Controls.Add(flowLayoutPanel7);
        flowLayoutPanel6.Controls.Add(flowLayoutPanel8);
        flowLayoutPanel6.Controls.Add(labelTP2_4);
        flowLayoutPanel6.FlowDirection = FlowDirection.TopDown;
        flowLayoutPanel6.Location = new Point(94, 10);
        flowLayoutPanel6.Margin = new Padding(2);
        flowLayoutPanel6.Name = "flowLayoutPanel6";
        flowLayoutPanel6.Padding = new Padding(2);
        flowLayoutPanel6.Size = new Size(68, 72);
        flowLayoutPanel6.TabIndex = 5;
        flowLayoutPanel6.WrapContents = false;
        // 
        // flowLayoutPanel7
        // 
        flowLayoutPanel7.AutoSize = true;
        flowLayoutPanel7.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel7.Controls.Add(buttonTP2_4_T_On);
        flowLayoutPanel7.Controls.Add(buttonTP2_4_C_On);
        flowLayoutPanel7.Location = new Point(3, 3);
        flowLayoutPanel7.Margin = new Padding(1);
        flowLayoutPanel7.Name = "flowLayoutPanel7";
        flowLayoutPanel7.Size = new Size(60, 22);
        flowLayoutPanel7.TabIndex = 0;
        flowLayoutPanel7.WrapContents = false;
        // 
        // buttonTP2_4_T_On
        // 
        buttonTP2_4_T_On.Font = new Font("Segoe UI", 8F);
        buttonTP2_4_T_On.Location = new Point(1, 1);
        buttonTP2_4_T_On.Margin = new Padding(1);
        buttonTP2_4_T_On.Name = "buttonTP2_4_T_On";
        buttonTP2_4_T_On.Size = new Size(28, 20);
        buttonTP2_4_T_On.TabIndex = 1;
        buttonTP2_4_T_On.Text = "T+";
        buttonTP2_4_T_On.UseVisualStyleBackColor = true;
        // 
        // buttonTP2_4_C_On
        // 
        buttonTP2_4_C_On.Font = new Font("Segoe UI", 8F);
        buttonTP2_4_C_On.Location = new Point(31, 1);
        buttonTP2_4_C_On.Margin = new Padding(1);
        buttonTP2_4_C_On.Name = "buttonTP2_4_C_On";
        buttonTP2_4_C_On.Size = new Size(28, 20);
        buttonTP2_4_C_On.TabIndex = 1;
        buttonTP2_4_C_On.Text = "C+";
        buttonTP2_4_C_On.UseVisualStyleBackColor = true;
        // 
        // flowLayoutPanel8
        // 
        flowLayoutPanel8.AutoSize = true;
        flowLayoutPanel8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel8.Controls.Add(buttonTP2_4_T_Off);
        flowLayoutPanel8.Controls.Add(buttonTP2_4_C_Off);
        flowLayoutPanel8.Location = new Point(3, 27);
        flowLayoutPanel8.Margin = new Padding(1);
        flowLayoutPanel8.Name = "flowLayoutPanel8";
        flowLayoutPanel8.Size = new Size(60, 22);
        flowLayoutPanel8.TabIndex = 1;
        flowLayoutPanel8.WrapContents = false;
        // 
        // buttonTP2_4_T_Off
        // 
        buttonTP2_4_T_Off.Font = new Font("Segoe UI", 8F);
        buttonTP2_4_T_Off.Location = new Point(1, 1);
        buttonTP2_4_T_Off.Margin = new Padding(1);
        buttonTP2_4_T_Off.Name = "buttonTP2_4_T_Off";
        buttonTP2_4_T_Off.Size = new Size(28, 20);
        buttonTP2_4_T_Off.TabIndex = 0;
        buttonTP2_4_T_Off.Text = "T-";
        buttonTP2_4_T_Off.UseVisualStyleBackColor = true;
        // 
        // buttonTP2_4_C_Off
        // 
        buttonTP2_4_C_Off.Font = new Font("Segoe UI", 8F);
        buttonTP2_4_C_Off.Location = new Point(31, 1);
        buttonTP2_4_C_Off.Margin = new Padding(1);
        buttonTP2_4_C_Off.Name = "buttonTP2_4_C_Off";
        buttonTP2_4_C_Off.Size = new Size(28, 20);
        buttonTP2_4_C_Off.TabIndex = 0;
        buttonTP2_4_C_Off.Text = "C-";
        buttonTP2_4_C_Off.UseVisualStyleBackColor = true;
        // 
        // labelTP2_4
        // 
        labelTP2_4.Font = new Font("Segoe UI", 8F);
        labelTP2_4.Location = new Point(2, 50);
        labelTP2_4.Margin = new Padding(0);
        labelTP2_4.Name = "labelTP2_4";
        labelTP2_4.Size = new Size(60, 18);
        labelTP2_4.TabIndex = 0;
        labelTP2_4.Text = "2/4";
        labelTP2_4.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // flowLayoutPanel9
        // 
        flowLayoutPanel9.AutoSize = true;
        flowLayoutPanel9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel9.BorderStyle = BorderStyle.FixedSingle;
        flowLayoutPanel9.Controls.Add(flowLayoutPanel10);
        flowLayoutPanel9.Controls.Add(flowLayoutPanel11);
        flowLayoutPanel9.Controls.Add(labelTP3);
        flowLayoutPanel9.FlowDirection = FlowDirection.TopDown;
        flowLayoutPanel9.Location = new Point(166, 10);
        flowLayoutPanel9.Margin = new Padding(2);
        flowLayoutPanel9.Name = "flowLayoutPanel9";
        flowLayoutPanel9.Padding = new Padding(2);
        flowLayoutPanel9.Size = new Size(68, 72);
        flowLayoutPanel9.TabIndex = 6;
        flowLayoutPanel9.WrapContents = false;
        // 
        // flowLayoutPanel10
        // 
        flowLayoutPanel10.AutoSize = true;
        flowLayoutPanel10.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel10.Controls.Add(buttonTP3_T_On);
        flowLayoutPanel10.Controls.Add(buttonTP3_C_On);
        flowLayoutPanel10.Location = new Point(3, 3);
        flowLayoutPanel10.Margin = new Padding(1);
        flowLayoutPanel10.Name = "flowLayoutPanel10";
        flowLayoutPanel10.Size = new Size(60, 22);
        flowLayoutPanel10.TabIndex = 0;
        flowLayoutPanel10.WrapContents = false;
        // 
        // buttonTP3_T_On
        // 
        buttonTP3_T_On.Font = new Font("Segoe UI", 8F);
        buttonTP3_T_On.Location = new Point(1, 1);
        buttonTP3_T_On.Margin = new Padding(1);
        buttonTP3_T_On.Name = "buttonTP3_T_On";
        buttonTP3_T_On.Size = new Size(28, 20);
        buttonTP3_T_On.TabIndex = 1;
        buttonTP3_T_On.Text = "T+";
        buttonTP3_T_On.UseVisualStyleBackColor = true;
        // 
        // buttonTP3_C_On
        // 
        buttonTP3_C_On.Font = new Font("Segoe UI", 8F);
        buttonTP3_C_On.Location = new Point(31, 1);
        buttonTP3_C_On.Margin = new Padding(1);
        buttonTP3_C_On.Name = "buttonTP3_C_On";
        buttonTP3_C_On.Size = new Size(28, 20);
        buttonTP3_C_On.TabIndex = 1;
        buttonTP3_C_On.Text = "C+";
        buttonTP3_C_On.UseVisualStyleBackColor = true;
        // 
        // flowLayoutPanel11
        // 
        flowLayoutPanel11.AutoSize = true;
        flowLayoutPanel11.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel11.Controls.Add(buttonTP3_T_Off);
        flowLayoutPanel11.Controls.Add(buttonTP3_C_Off);
        flowLayoutPanel11.Location = new Point(3, 27);
        flowLayoutPanel11.Margin = new Padding(1);
        flowLayoutPanel11.Name = "flowLayoutPanel11";
        flowLayoutPanel11.Size = new Size(60, 22);
        flowLayoutPanel11.TabIndex = 1;
        flowLayoutPanel11.WrapContents = false;
        // 
        // buttonTP3_T_Off
        // 
        buttonTP3_T_Off.Font = new Font("Segoe UI", 8F);
        buttonTP3_T_Off.Location = new Point(1, 1);
        buttonTP3_T_Off.Margin = new Padding(1);
        buttonTP3_T_Off.Name = "buttonTP3_T_Off";
        buttonTP3_T_Off.Size = new Size(28, 20);
        buttonTP3_T_Off.TabIndex = 0;
        buttonTP3_T_Off.Text = "T-";
        buttonTP3_T_Off.UseVisualStyleBackColor = true;
        // 
        // buttonTP3_C_Off
        // 
        buttonTP3_C_Off.Font = new Font("Segoe UI", 8F);
        buttonTP3_C_Off.Location = new Point(31, 1);
        buttonTP3_C_Off.Margin = new Padding(1);
        buttonTP3_C_Off.Name = "buttonTP3_C_Off";
        buttonTP3_C_Off.Size = new Size(28, 20);
        buttonTP3_C_Off.TabIndex = 0;
        buttonTP3_C_Off.Text = "C-";
        buttonTP3_C_Off.UseVisualStyleBackColor = true;
        // 
        // labelTP3
        // 
        labelTP3.Font = new Font("Segoe UI", 8F);
        labelTP3.Location = new Point(2, 50);
        labelTP3.Margin = new Padding(0);
        labelTP3.Name = "labelTP3";
        labelTP3.Size = new Size(60, 18);
        labelTP3.TabIndex = 0;
        labelTP3.Text = "3";
        labelTP3.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // flowLayoutPanel12
        // 
        flowLayoutPanel12.AutoSize = true;
        flowLayoutPanel12.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel12.BorderStyle = BorderStyle.FixedSingle;
        flowLayoutPanel12.Controls.Add(flowLayoutPanel13);
        flowLayoutPanel12.Controls.Add(flowLayoutPanel14);
        flowLayoutPanel12.Controls.Add(labelTP5_7);
        flowLayoutPanel12.FlowDirection = FlowDirection.TopDown;
        flowLayoutPanel12.Location = new Point(238, 10);
        flowLayoutPanel12.Margin = new Padding(2);
        flowLayoutPanel12.Name = "flowLayoutPanel12";
        flowLayoutPanel12.Padding = new Padding(2);
        flowLayoutPanel12.Size = new Size(68, 72);
        flowLayoutPanel12.TabIndex = 7;
        flowLayoutPanel12.WrapContents = false;
        // 
        // flowLayoutPanel13
        // 
        flowLayoutPanel13.AutoSize = true;
        flowLayoutPanel13.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel13.Controls.Add(buttonTP5_7_T_On);
        flowLayoutPanel13.Controls.Add(buttonTP5_7_C_On);
        flowLayoutPanel13.Location = new Point(3, 3);
        flowLayoutPanel13.Margin = new Padding(1);
        flowLayoutPanel13.Name = "flowLayoutPanel13";
        flowLayoutPanel13.Size = new Size(60, 22);
        flowLayoutPanel13.TabIndex = 0;
        flowLayoutPanel13.WrapContents = false;
        // 
        // buttonTP5_7_T_On
        // 
        buttonTP5_7_T_On.Font = new Font("Segoe UI", 8F);
        buttonTP5_7_T_On.Location = new Point(1, 1);
        buttonTP5_7_T_On.Margin = new Padding(1);
        buttonTP5_7_T_On.Name = "buttonTP5_7_T_On";
        buttonTP5_7_T_On.Size = new Size(28, 20);
        buttonTP5_7_T_On.TabIndex = 1;
        buttonTP5_7_T_On.Text = "T+";
        buttonTP5_7_T_On.UseVisualStyleBackColor = true;
        // 
        // buttonTP5_7_C_On
        // 
        buttonTP5_7_C_On.Font = new Font("Segoe UI", 8F);
        buttonTP5_7_C_On.Location = new Point(31, 1);
        buttonTP5_7_C_On.Margin = new Padding(1);
        buttonTP5_7_C_On.Name = "buttonTP5_7_C_On";
        buttonTP5_7_C_On.Size = new Size(28, 20);
        buttonTP5_7_C_On.TabIndex = 1;
        buttonTP5_7_C_On.Text = "C+";
        buttonTP5_7_C_On.UseVisualStyleBackColor = true;
        // 
        // flowLayoutPanel14
        // 
        flowLayoutPanel14.AutoSize = true;
        flowLayoutPanel14.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel14.Controls.Add(buttonTP5_7_T_Off);
        flowLayoutPanel14.Controls.Add(buttonTP5_7_C_Off);
        flowLayoutPanel14.Location = new Point(3, 27);
        flowLayoutPanel14.Margin = new Padding(1);
        flowLayoutPanel14.Name = "flowLayoutPanel14";
        flowLayoutPanel14.Size = new Size(60, 22);
        flowLayoutPanel14.TabIndex = 1;
        flowLayoutPanel14.WrapContents = false;
        // 
        // buttonTP5_7_T_Off
        // 
        buttonTP5_7_T_Off.Font = new Font("Segoe UI", 8F);
        buttonTP5_7_T_Off.Location = new Point(1, 1);
        buttonTP5_7_T_Off.Margin = new Padding(1);
        buttonTP5_7_T_Off.Name = "buttonTP5_7_T_Off";
        buttonTP5_7_T_Off.Size = new Size(28, 20);
        buttonTP5_7_T_Off.TabIndex = 0;
        buttonTP5_7_T_Off.Text = "T-";
        buttonTP5_7_T_Off.UseVisualStyleBackColor = true;
        // 
        // buttonTP5_7_C_Off
        // 
        buttonTP5_7_C_Off.Font = new Font("Segoe UI", 8F);
        buttonTP5_7_C_Off.Location = new Point(31, 1);
        buttonTP5_7_C_Off.Margin = new Padding(1);
        buttonTP5_7_C_Off.Name = "buttonTP5_7_C_Off";
        buttonTP5_7_C_Off.Size = new Size(28, 20);
        buttonTP5_7_C_Off.TabIndex = 0;
        buttonTP5_7_C_Off.Text = "C-";
        buttonTP5_7_C_Off.UseVisualStyleBackColor = true;
        // 
        // labelTP5_7
        // 
        labelTP5_7.Font = new Font("Segoe UI", 8F);
        labelTP5_7.Location = new Point(2, 50);
        labelTP5_7.Margin = new Padding(0);
        labelTP5_7.Name = "labelTP5_7";
        labelTP5_7.Size = new Size(60, 18);
        labelTP5_7.TabIndex = 0;
        labelTP5_7.Text = "5/7";
        labelTP5_7.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // flowLayoutPanel15
        // 
        flowLayoutPanel15.AutoSize = true;
        flowLayoutPanel15.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel15.BorderStyle = BorderStyle.FixedSingle;
        flowLayoutPanel15.Controls.Add(flowLayoutPanel16);
        flowLayoutPanel15.Controls.Add(flowLayoutPanel17);
        flowLayoutPanel15.Controls.Add(labelTP6_8);
        flowLayoutPanel15.FlowDirection = FlowDirection.TopDown;
        flowLayoutPanel15.Location = new Point(310, 10);
        flowLayoutPanel15.Margin = new Padding(2);
        flowLayoutPanel15.Name = "flowLayoutPanel15";
        flowLayoutPanel15.Padding = new Padding(2);
        flowLayoutPanel15.Size = new Size(68, 72);
        flowLayoutPanel15.TabIndex = 8;
        flowLayoutPanel15.WrapContents = false;
        // 
        // flowLayoutPanel16
        // 
        flowLayoutPanel16.AutoSize = true;
        flowLayoutPanel16.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel16.Controls.Add(buttonTP6_8_T_On);
        flowLayoutPanel16.Controls.Add(buttonTP6_8_C_On);
        flowLayoutPanel16.Location = new Point(3, 3);
        flowLayoutPanel16.Margin = new Padding(1);
        flowLayoutPanel16.Name = "flowLayoutPanel16";
        flowLayoutPanel16.Size = new Size(60, 22);
        flowLayoutPanel16.TabIndex = 0;
        flowLayoutPanel16.WrapContents = false;
        // 
        // buttonTP6_8_T_On
        // 
        buttonTP6_8_T_On.Font = new Font("Segoe UI", 8F);
        buttonTP6_8_T_On.Location = new Point(1, 1);
        buttonTP6_8_T_On.Margin = new Padding(1);
        buttonTP6_8_T_On.Name = "buttonTP6_8_T_On";
        buttonTP6_8_T_On.Size = new Size(28, 20);
        buttonTP6_8_T_On.TabIndex = 1;
        buttonTP6_8_T_On.Text = "T+";
        buttonTP6_8_T_On.UseVisualStyleBackColor = true;
        // 
        // buttonTP6_8_C_On
        // 
        buttonTP6_8_C_On.Font = new Font("Segoe UI", 8F);
        buttonTP6_8_C_On.Location = new Point(31, 1);
        buttonTP6_8_C_On.Margin = new Padding(1);
        buttonTP6_8_C_On.Name = "buttonTP6_8_C_On";
        buttonTP6_8_C_On.Size = new Size(28, 20);
        buttonTP6_8_C_On.TabIndex = 1;
        buttonTP6_8_C_On.Text = "C+";
        buttonTP6_8_C_On.UseVisualStyleBackColor = true;
        // 
        // flowLayoutPanel17
        // 
        flowLayoutPanel17.AutoSize = true;
        flowLayoutPanel17.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel17.Controls.Add(buttonTP6_8_T_Off);
        flowLayoutPanel17.Controls.Add(buttonTP6_8_C_Off);
        flowLayoutPanel17.Location = new Point(3, 27);
        flowLayoutPanel17.Margin = new Padding(1);
        flowLayoutPanel17.Name = "flowLayoutPanel17";
        flowLayoutPanel17.Size = new Size(60, 22);
        flowLayoutPanel17.TabIndex = 1;
        flowLayoutPanel17.WrapContents = false;
        // 
        // buttonTP6_8_T_Off
        // 
        buttonTP6_8_T_Off.Font = new Font("Segoe UI", 8F);
        buttonTP6_8_T_Off.Location = new Point(1, 1);
        buttonTP6_8_T_Off.Margin = new Padding(1);
        buttonTP6_8_T_Off.Name = "buttonTP6_8_T_Off";
        buttonTP6_8_T_Off.Size = new Size(28, 20);
        buttonTP6_8_T_Off.TabIndex = 0;
        buttonTP6_8_T_Off.Text = "T-";
        buttonTP6_8_T_Off.UseVisualStyleBackColor = true;
        // 
        // buttonTP6_8_C_Off
        // 
        buttonTP6_8_C_Off.Font = new Font("Segoe UI", 8F);
        buttonTP6_8_C_Off.Location = new Point(31, 1);
        buttonTP6_8_C_Off.Margin = new Padding(1);
        buttonTP6_8_C_Off.Name = "buttonTP6_8_C_Off";
        buttonTP6_8_C_Off.Size = new Size(28, 20);
        buttonTP6_8_C_Off.TabIndex = 0;
        buttonTP6_8_C_Off.Text = "C-";
        buttonTP6_8_C_Off.UseVisualStyleBackColor = true;
        // 
        // labelTP6_8
        // 
        labelTP6_8.Font = new Font("Segoe UI", 8F);
        labelTP6_8.Location = new Point(2, 50);
        labelTP6_8.Margin = new Padding(0);
        labelTP6_8.Name = "labelTP6_8";
        labelTP6_8.Size = new Size(60, 18);
        labelTP6_8.TabIndex = 0;
        labelTP6_8.Text = "6/8";
        labelTP6_8.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // flowLayoutPanel18
        // 
        flowLayoutPanel18.AutoSize = true;
        flowLayoutPanel18.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel18.BorderStyle = BorderStyle.FixedSingle;
        flowLayoutPanel18.Controls.Add(flowLayoutPanel19);
        flowLayoutPanel18.Controls.Add(flowLayoutPanel20);
        flowLayoutPanel18.Controls.Add(labelTP9);
        flowLayoutPanel18.FlowDirection = FlowDirection.TopDown;
        flowLayoutPanel18.Location = new Point(382, 10);
        flowLayoutPanel18.Margin = new Padding(2);
        flowLayoutPanel18.Name = "flowLayoutPanel18";
        flowLayoutPanel18.Padding = new Padding(2);
        flowLayoutPanel18.Size = new Size(68, 72);
        flowLayoutPanel18.TabIndex = 9;
        flowLayoutPanel18.WrapContents = false;
        // 
        // flowLayoutPanel19
        // 
        flowLayoutPanel19.AutoSize = true;
        flowLayoutPanel19.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel19.Controls.Add(buttonTP9_T_On);
        flowLayoutPanel19.Controls.Add(buttonTP9_C_On);
        flowLayoutPanel19.Location = new Point(3, 3);
        flowLayoutPanel19.Margin = new Padding(1);
        flowLayoutPanel19.Name = "flowLayoutPanel19";
        flowLayoutPanel19.Size = new Size(60, 22);
        flowLayoutPanel19.TabIndex = 0;
        flowLayoutPanel19.WrapContents = false;
        // 
        // buttonTP9_T_On
        // 
        buttonTP9_T_On.Font = new Font("Segoe UI", 8F);
        buttonTP9_T_On.Location = new Point(1, 1);
        buttonTP9_T_On.Margin = new Padding(1);
        buttonTP9_T_On.Name = "buttonTP9_T_On";
        buttonTP9_T_On.Size = new Size(28, 20);
        buttonTP9_T_On.TabIndex = 1;
        buttonTP9_T_On.Text = "T+";
        buttonTP9_T_On.UseVisualStyleBackColor = true;
        // 
        // buttonTP9_C_On
        // 
        buttonTP9_C_On.Font = new Font("Segoe UI", 8F);
        buttonTP9_C_On.Location = new Point(31, 1);
        buttonTP9_C_On.Margin = new Padding(1);
        buttonTP9_C_On.Name = "buttonTP9_C_On";
        buttonTP9_C_On.Size = new Size(28, 20);
        buttonTP9_C_On.TabIndex = 1;
        buttonTP9_C_On.Text = "C+";
        buttonTP9_C_On.UseVisualStyleBackColor = true;
        // 
        // flowLayoutPanel20
        // 
        flowLayoutPanel20.AutoSize = true;
        flowLayoutPanel20.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel20.Controls.Add(buttonTP9_T_Off);
        flowLayoutPanel20.Controls.Add(buttonTP9_C_Off);
        flowLayoutPanel20.Location = new Point(3, 27);
        flowLayoutPanel20.Margin = new Padding(1);
        flowLayoutPanel20.Name = "flowLayoutPanel20";
        flowLayoutPanel20.Size = new Size(60, 22);
        flowLayoutPanel20.TabIndex = 1;
        flowLayoutPanel20.WrapContents = false;
        // 
        // buttonTP9_T_Off
        // 
        buttonTP9_T_Off.Font = new Font("Segoe UI", 8F);
        buttonTP9_T_Off.Location = new Point(1, 1);
        buttonTP9_T_Off.Margin = new Padding(1);
        buttonTP9_T_Off.Name = "buttonTP9_T_Off";
        buttonTP9_T_Off.Size = new Size(28, 20);
        buttonTP9_T_Off.TabIndex = 0;
        buttonTP9_T_Off.Text = "T-";
        buttonTP9_T_Off.UseVisualStyleBackColor = true;
        // 
        // buttonTP9_C_Off
        // 
        buttonTP9_C_Off.Font = new Font("Segoe UI", 8F);
        buttonTP9_C_Off.Location = new Point(31, 1);
        buttonTP9_C_Off.Margin = new Padding(1);
        buttonTP9_C_Off.Name = "buttonTP9_C_Off";
        buttonTP9_C_Off.Size = new Size(28, 20);
        buttonTP9_C_Off.TabIndex = 0;
        buttonTP9_C_Off.Text = "C-";
        buttonTP9_C_Off.UseVisualStyleBackColor = true;
        // 
        // labelTP9
        // 
        labelTP9.Font = new Font("Segoe UI", 8F);
        labelTP9.Location = new Point(2, 50);
        labelTP9.Margin = new Padding(0);
        labelTP9.Name = "labelTP9";
        labelTP9.Size = new Size(60, 18);
        labelTP9.TabIndex = 0;
        labelTP9.Text = "9";
        labelTP9.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // flowLayoutPanel21
        // 
        flowLayoutPanel21.AutoSize = true;
        flowLayoutPanel21.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel21.BorderStyle = BorderStyle.FixedSingle;
        flowLayoutPanel21.Controls.Add(flowLayoutPanel22);
        flowLayoutPanel21.Controls.Add(flowLayoutPanel23);
        flowLayoutPanel21.Controls.Add(labelTP10);
        flowLayoutPanel21.FlowDirection = FlowDirection.TopDown;
        flowLayoutPanel21.Location = new Point(454, 10);
        flowLayoutPanel21.Margin = new Padding(2);
        flowLayoutPanel21.Name = "flowLayoutPanel21";
        flowLayoutPanel21.Padding = new Padding(2);
        flowLayoutPanel21.Size = new Size(68, 72);
        flowLayoutPanel21.TabIndex = 10;
        flowLayoutPanel21.WrapContents = false;
        // 
        // flowLayoutPanel22
        // 
        flowLayoutPanel22.AutoSize = true;
        flowLayoutPanel22.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel22.Controls.Add(buttonTP10_T_On);
        flowLayoutPanel22.Controls.Add(buttonTP10_C_On);
        flowLayoutPanel22.Location = new Point(3, 3);
        flowLayoutPanel22.Margin = new Padding(1);
        flowLayoutPanel22.Name = "flowLayoutPanel22";
        flowLayoutPanel22.Size = new Size(60, 22);
        flowLayoutPanel22.TabIndex = 0;
        flowLayoutPanel22.WrapContents = false;
        // 
        // buttonTP10_T_On
        // 
        buttonTP10_T_On.Font = new Font("Segoe UI", 8F);
        buttonTP10_T_On.Location = new Point(1, 1);
        buttonTP10_T_On.Margin = new Padding(1);
        buttonTP10_T_On.Name = "buttonTP10_T_On";
        buttonTP10_T_On.Size = new Size(28, 20);
        buttonTP10_T_On.TabIndex = 1;
        buttonTP10_T_On.Text = "T+";
        buttonTP10_T_On.UseVisualStyleBackColor = true;
        // 
        // buttonTP10_C_On
        // 
        buttonTP10_C_On.Font = new Font("Segoe UI", 8F);
        buttonTP10_C_On.Location = new Point(31, 1);
        buttonTP10_C_On.Margin = new Padding(1);
        buttonTP10_C_On.Name = "buttonTP10_C_On";
        buttonTP10_C_On.Size = new Size(28, 20);
        buttonTP10_C_On.TabIndex = 1;
        buttonTP10_C_On.Text = "C+";
        buttonTP10_C_On.UseVisualStyleBackColor = true;
        // 
        // flowLayoutPanel23
        // 
        flowLayoutPanel23.AutoSize = true;
        flowLayoutPanel23.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel23.Controls.Add(buttonTP10_T_Off);
        flowLayoutPanel23.Controls.Add(buttonTP10_C_Off);
        flowLayoutPanel23.Location = new Point(3, 27);
        flowLayoutPanel23.Margin = new Padding(1);
        flowLayoutPanel23.Name = "flowLayoutPanel23";
        flowLayoutPanel23.Size = new Size(60, 22);
        flowLayoutPanel23.TabIndex = 1;
        flowLayoutPanel23.WrapContents = false;
        // 
        // buttonTP10_T_Off
        // 
        buttonTP10_T_Off.Font = new Font("Segoe UI", 8F);
        buttonTP10_T_Off.Location = new Point(1, 1);
        buttonTP10_T_Off.Margin = new Padding(1);
        buttonTP10_T_Off.Name = "buttonTP10_T_Off";
        buttonTP10_T_Off.Size = new Size(28, 20);
        buttonTP10_T_Off.TabIndex = 0;
        buttonTP10_T_Off.Text = "T-";
        buttonTP10_T_Off.UseVisualStyleBackColor = true;
        // 
        // buttonTP10_C_Off
        // 
        buttonTP10_C_Off.Font = new Font("Segoe UI", 8F);
        buttonTP10_C_Off.Location = new Point(31, 1);
        buttonTP10_C_Off.Margin = new Padding(1);
        buttonTP10_C_Off.Name = "buttonTP10_C_Off";
        buttonTP10_C_Off.Size = new Size(28, 20);
        buttonTP10_C_Off.TabIndex = 0;
        buttonTP10_C_Off.Text = "C-";
        buttonTP10_C_Off.UseVisualStyleBackColor = true;
        // 
        // labelTP10
        // 
        labelTP10.Font = new Font("Segoe UI", 8F);
        labelTP10.Location = new Point(2, 50);
        labelTP10.Margin = new Padding(0);
        labelTP10.Name = "labelTP10";
        labelTP10.Size = new Size(60, 18);
        labelTP10.TabIndex = 0;
        labelTP10.Text = "10";
        labelTP10.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // flowLayoutPanel24
        // 
        flowLayoutPanel24.AutoSize = true;
        flowLayoutPanel24.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel24.BorderStyle = BorderStyle.FixedSingle;
        flowLayoutPanel24.Controls.Add(flowLayoutPanel25);
        flowLayoutPanel24.Controls.Add(flowLayoutPanel26);
        flowLayoutPanel24.Controls.Add(labelTP12);
        flowLayoutPanel24.FlowDirection = FlowDirection.TopDown;
        flowLayoutPanel24.Location = new Point(526, 10);
        flowLayoutPanel24.Margin = new Padding(2);
        flowLayoutPanel24.Name = "flowLayoutPanel24";
        flowLayoutPanel24.Padding = new Padding(2);
        flowLayoutPanel24.Size = new Size(68, 72);
        flowLayoutPanel24.TabIndex = 11;
        flowLayoutPanel24.WrapContents = false;
        // 
        // flowLayoutPanel25
        // 
        flowLayoutPanel25.AutoSize = true;
        flowLayoutPanel25.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel25.Controls.Add(buttonTP12_T_On);
        flowLayoutPanel25.Controls.Add(buttonTP12_C_On);
        flowLayoutPanel25.Location = new Point(3, 3);
        flowLayoutPanel25.Margin = new Padding(1);
        flowLayoutPanel25.Name = "flowLayoutPanel25";
        flowLayoutPanel25.Size = new Size(60, 22);
        flowLayoutPanel25.TabIndex = 0;
        flowLayoutPanel25.WrapContents = false;
        // 
        // buttonTP12_T_On
        // 
        buttonTP12_T_On.Font = new Font("Segoe UI", 8F);
        buttonTP12_T_On.Location = new Point(1, 1);
        buttonTP12_T_On.Margin = new Padding(1);
        buttonTP12_T_On.Name = "buttonTP12_T_On";
        buttonTP12_T_On.Size = new Size(28, 20);
        buttonTP12_T_On.TabIndex = 1;
        buttonTP12_T_On.Text = "T+";
        buttonTP12_T_On.UseVisualStyleBackColor = true;
        // 
        // buttonTP12_C_On
        // 
        buttonTP12_C_On.Font = new Font("Segoe UI", 8F);
        buttonTP12_C_On.Location = new Point(31, 1);
        buttonTP12_C_On.Margin = new Padding(1);
        buttonTP12_C_On.Name = "buttonTP12_C_On";
        buttonTP12_C_On.Size = new Size(28, 20);
        buttonTP12_C_On.TabIndex = 1;
        buttonTP12_C_On.Text = "C+";
        buttonTP12_C_On.UseVisualStyleBackColor = true;
        // 
        // flowLayoutPanel26
        // 
        flowLayoutPanel26.AutoSize = true;
        flowLayoutPanel26.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flowLayoutPanel26.Controls.Add(buttonTP12_T_Off);
        flowLayoutPanel26.Controls.Add(buttonTP12_C_Off);
        flowLayoutPanel26.Location = new Point(3, 27);
        flowLayoutPanel26.Margin = new Padding(1);
        flowLayoutPanel26.Name = "flowLayoutPanel26";
        flowLayoutPanel26.Size = new Size(60, 22);
        flowLayoutPanel26.TabIndex = 1;
        flowLayoutPanel26.WrapContents = false;
        // 
        // buttonTP12_T_Off
        // 
        buttonTP12_T_Off.Font = new Font("Segoe UI", 8F);
        buttonTP12_T_Off.Location = new Point(1, 1);
        buttonTP12_T_Off.Margin = new Padding(1);
        buttonTP12_T_Off.Name = "buttonTP12_T_Off";
        buttonTP12_T_Off.Size = new Size(28, 20);
        buttonTP12_T_Off.TabIndex = 0;
        buttonTP12_T_Off.Text = "T-";
        buttonTP12_T_Off.UseVisualStyleBackColor = true;
        // 
        // buttonTP12_C_Off
        // 
        buttonTP12_C_Off.Font = new Font("Segoe UI", 8F);
        buttonTP12_C_Off.Location = new Point(31, 1);
        buttonTP12_C_Off.Margin = new Padding(1);
        buttonTP12_C_Off.Name = "buttonTP12_C_Off";
        buttonTP12_C_Off.Size = new Size(28, 20);
        buttonTP12_C_Off.TabIndex = 0;
        buttonTP12_C_Off.Text = "C-";
        buttonTP12_C_Off.UseVisualStyleBackColor = true;
        // 
        // labelTP12
        // 
        labelTP12.Font = new Font("Segoe UI", 8F);
        labelTP12.Location = new Point(2, 50);
        labelTP12.Margin = new Padding(0);
        labelTP12.Name = "labelTP12";
        labelTP12.Size = new Size(60, 18);
        labelTP12.TabIndex = 0;
        labelTP12.Text = "12";
        labelTP12.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _contentPanel
        // 
        _contentPanel.ColumnCount = 1;
        _contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _contentPanel.Controls.Add(textBox_log, 0, 0);
        _contentPanel.Dock = DockStyle.Fill;
        _contentPanel.Location = new Point(0, 110);
        _contentPanel.Margin = new Padding(0);
        _contentPanel.Name = "_contentPanel";
        _contentPanel.RowCount = 2;
        _contentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _contentPanel.RowStyles.Add(new RowStyle());
        _contentPanel.Size = new Size(1379, 638);
        _contentPanel.TabIndex = 1;
        // 
        // _commandPanel
        // 
        _commandPanel.AutoSize = true;
        _commandPanel.ColumnCount = 2;
        _commandPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _commandPanel.ColumnStyles.Add(new ColumnStyle());
        _commandPanel.Controls.Add(textBox_cmd, 0, 0);
        _commandPanel.Controls.Add(sendButton, 1, 0);
        _commandPanel.Dock = DockStyle.Bottom;
        _commandPanel.Location = new Point(0, 748);
        _commandPanel.Name = "_commandPanel";
        _commandPanel.Padding = new Padding(8);
        _commandPanel.RowCount = 2;
        _commandPanel.RowStyles.Add(new RowStyle());
        _commandPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        _commandPanel.Size = new Size(1379, 67);
        _commandPanel.TabIndex = 2;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1379, 815);
        Controls.Add(_contentPanel);
        Controls.Add(_commandPanel);
        Controls.Add(_buttonPanel);
        MinimumSize = new Size(900, 600);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Z21_TC2026 launcher";
        _buttonPanel.ResumeLayout(false);
        _buttonPanel.PerformLayout();
        _rightButtonPanel.ResumeLayout(false);
        _rightButtonPanel.PerformLayout();
        flowLayoutPanel3.ResumeLayout(false);
        flowLayoutPanel3.PerformLayout();
        flowLayoutPanel4.ResumeLayout(false);
        flowLayoutPanel5.ResumeLayout(false);
        flowLayoutPanel6.ResumeLayout(false);
        flowLayoutPanel6.PerformLayout();
        flowLayoutPanel7.ResumeLayout(false);
        flowLayoutPanel8.ResumeLayout(false);
        flowLayoutPanel9.ResumeLayout(false);
        flowLayoutPanel9.PerformLayout();
        flowLayoutPanel10.ResumeLayout(false);
        flowLayoutPanel11.ResumeLayout(false);
        flowLayoutPanel12.ResumeLayout(false);
        flowLayoutPanel12.PerformLayout();
        flowLayoutPanel13.ResumeLayout(false);
        flowLayoutPanel14.ResumeLayout(false);
        flowLayoutPanel15.ResumeLayout(false);
        flowLayoutPanel15.PerformLayout();
        flowLayoutPanel16.ResumeLayout(false);
        flowLayoutPanel17.ResumeLayout(false);
        flowLayoutPanel18.ResumeLayout(false);
        flowLayoutPanel18.PerformLayout();
        flowLayoutPanel19.ResumeLayout(false);
        flowLayoutPanel20.ResumeLayout(false);
        flowLayoutPanel21.ResumeLayout(false);
        flowLayoutPanel21.PerformLayout();
        flowLayoutPanel22.ResumeLayout(false);
        flowLayoutPanel23.ResumeLayout(false);
        flowLayoutPanel24.ResumeLayout(false);
        flowLayoutPanel24.PerformLayout();
        flowLayoutPanel25.ResumeLayout(false);
        flowLayoutPanel26.ResumeLayout(false);
        _contentPanel.ResumeLayout(false);
        _commandPanel.ResumeLayout(false);
        _commandPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private RichTextBox textBox_log;
    private TextBox textBox_cmd;
    private Button sendButton;
    private Button helpButton;
    private Button clearButton;
    private CheckBox serverCheckBox;
    private CheckBox debugCheckBox;
    private FlowLayoutPanel _buttonPanel;
    private TableLayoutPanel _contentPanel;
    private FlowLayoutPanel _rightButtonPanel;
    private TableLayoutPanel _commandPanel;
    private Button button_powerOn;
    private Button button_powerOff;
    private Button buttonTP1_C_Off;
    private Button buttonTP1_C_On;
    private FlowLayoutPanel flowLayoutPanel1;
    private FlowLayoutPanel flowLayoutPanel2;
    private Button buttonTP1_T_On;
    private Button buttonTP1_T_Off;
    private FlowLayoutPanel flowLayoutPanel3;
    private FlowLayoutPanel flowLayoutPanel4;
    private FlowLayoutPanel flowLayoutPanel5;
    private Label labelTP1;
    private FlowLayoutPanel flowLayoutPanel6;
    private FlowLayoutPanel flowLayoutPanel7;
    private Button buttonTP2_4_C_On;
    private Button buttonTP2_4_C_Off;
    private FlowLayoutPanel flowLayoutPanel8;
    private Label labelTP2_4;
    private Button buttonTP2_4_T_On;
    private Button buttonTP2_4_T_Off;
    private FlowLayoutPanel flowLayoutPanel9;
    private FlowLayoutPanel flowLayoutPanel10;
    private Button buttonTP3_C_On;
    private Button buttonTP3_C_Off;
    private FlowLayoutPanel flowLayoutPanel11;
    private Label labelTP3;
    private Button buttonTP3_T_On;
    private Button buttonTP3_T_Off;
    private FlowLayoutPanel flowLayoutPanel12;
    private FlowLayoutPanel flowLayoutPanel13;
    private Button buttonTP5_7_C_On;
    private Button buttonTP5_7_C_Off;
    private FlowLayoutPanel flowLayoutPanel14;
    private Label labelTP5_7;
    private Button buttonTP5_7_T_On;
    private Button buttonTP5_7_T_Off;
    private FlowLayoutPanel flowLayoutPanel15;
    private FlowLayoutPanel flowLayoutPanel16;
    private Button buttonTP6_8_C_On;
    private Button buttonTP6_8_C_Off;
    private FlowLayoutPanel flowLayoutPanel17;
    private Label labelTP6_8;
    private Button buttonTP6_8_T_On;
    private Button buttonTP6_8_T_Off;
    private FlowLayoutPanel flowLayoutPanel18;
    private FlowLayoutPanel flowLayoutPanel19;
    private Button buttonTP9_C_On;
    private Button buttonTP9_C_Off;
    private FlowLayoutPanel flowLayoutPanel20;
    private Label labelTP9;
    private Button buttonTP9_T_On;
    private Button buttonTP9_T_Off;
    private FlowLayoutPanel flowLayoutPanel21;
    private FlowLayoutPanel flowLayoutPanel22;
    private Button buttonTP10_C_On;
    private Button buttonTP10_C_Off;
    private FlowLayoutPanel flowLayoutPanel23;
    private Label labelTP10;
    private Button buttonTP10_T_On;
    private Button buttonTP10_T_Off;
    private FlowLayoutPanel flowLayoutPanel24;
    private FlowLayoutPanel flowLayoutPanel25;
    private Button buttonTP12_C_On;
    private Button buttonTP12_C_Off;
    private FlowLayoutPanel flowLayoutPanel26;
    private Label labelTP12;
    private Button buttonTP12_T_On;
    private Button buttonTP12_T_Off;
}
