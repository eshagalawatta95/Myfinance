﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyFinance.Entities;
using MyFinance.Enums;
using MyFinance.Core.Service;

namespace MyFinance.Views.UserControls.Transaction
{
    public partial class ManageTransactionUserControl : UserControl
    {
        private TransactionEntity _transactionEntity;
        private SheduledTransactionList _schtransactionEntity;
        private Action<ContentItemEnum> _changeContentMainFormAction;
        private IApplicationService _applicationService;
        private BindingList<TransactionPartyBinder> _transactionParties;

        public ManageTransactionUserControl(Action<ContentItemEnum> changeContentMainFormAction)
        {
            _changeContentMainFormAction = changeContentMainFormAction;
            _applicationService = MyFinance.Entities.MyFinanceApplication.DependancyContainer.GetInstance<IApplicationService>();
            _transactionEntity = null;
            InitializeComponent();
            comboBoxRepeat.DataSource = Enum.GetValues(typeof(ContentRepeatItemEnum));
            comboBoxType.SelectedIndex = 0;
            comboBoxRepeat.SelectedIndex = 1;
            dateTimePickerTo.MinDate = DateTime.Today.AddDays(1);
            dateTimePickerForm.MinDate = DateTime.Today;
            dateTimePickerOneTimeTransactionDate.Value = DateTime.Now;
            dateTimePickerOneTimeTransactionDate.MaxDate= DateTime.Now;
            panel11.Visible = true;
            referenceNumberPanel.Visible = false;
        }

        public ManageTransactionUserControl(Action<ContentItemEnum> changeContent, TransactionEntity transactionEntity, SheduledTransactionList schtrans) : this(changeContent)
        {
            _transactionEntity = transactionEntity;
            if(schtrans != null)
            _schtransactionEntity = schtrans;
        }

        private void SetTransactionEntity(TransactionEntity transactionEntity = null)
        {
            _transactionEntity = transactionEntity ?? new TransactionEntity()
            {
                IsIncome = true,
                Amount = 0D,
                ReferenceNumber = "",
                Remarks = "",
                TransactionPartyId = 0
            };

            if (_transactionEntity.Id == 0)
            {
                referenceNumberPanel.Visible = false;
                transactionPartyComboBox.SelectedIndex = -1;
            }
            else
            {
                referenceNumberPanel.Visible = true;
                referenceNumberTextBox.Text = _transactionEntity.ReferenceNumber;
                transactionPartyComboBox.SelectedItem = _transactionParties.First(tp => tp.Id == _transactionEntity.TransactionPartyId);
            }

            transactionPartErrorLabel.Text = "";

            amountNumericUpDown.Text = _transactionEntity.Amount.ToString();
            amountErrorLabel.Text = "";
            remarksTextBox.Text = _transactionEntity.Remarks;
            remarksErrorLabel.Text = "";
            incomeCheckBox.Checked = _transactionEntity.IsIncome;
            comboBoxType.Enabled = false;
            panel11.Visible = true;
        }

        private void SetScheduleTransactionEntity(SheduledTransactionList transactionEntity = null)
        {
            _schtransactionEntity = transactionEntity ?? new SheduledTransactionList()
            {
                IsIncome = true,
                Amount = 0D,
                ReferenceNumber = "",
                Remarks = "",
                TransactionPartyId = 0
            };

            if (_schtransactionEntity.Id == 0)
            {
                referenceNumberPanel.Visible = false;
                transactionPartyComboBox.SelectedIndex = -1;
            }
            else
            {
                comboBoxType.SelectedIndex = 1;
                comboBoxType.Enabled = false;
                referenceNumberPanel.Visible = true;
                referenceNumberTextBox.Text = _schtransactionEntity.ReferenceNumber;
                transactionPartyComboBox.SelectedItem = _transactionParties.First(tp => tp.Id == _schtransactionEntity.TransactionPartyId);
            }

            transactionPartErrorLabel.Text = "";

            amountNumericUpDown.Text = _schtransactionEntity.Amount.ToString();
            amountErrorLabel.Text = "";
            remarksTextBox.Text = _schtransactionEntity.Remarks;
            remarksErrorLabel.Text = "";
            incomeCheckBox.Checked = _schtransactionEntity.IsIncome;

            panel6.Visible = true;
            panel7.Visible = true;
            panel8.Visible = true;
            panel9.Visible = true;
            panel11.Visible = false;
            DateTime dtTemp = DateTime.Now;

            comboBoxRepeat.SelectedIndex = comboBoxRepeat.FindStringExact(_schtransactionEntity.RepeatType);
            checkBoxNever.Checked = _schtransactionEntity.InfiniteSchedule;
            checkBoxActive.Checked = _schtransactionEntity.IsActive;
            dateTimePickerForm.Value = DateTime.Today.AddDays(1);
            if (checkBoxNever.Checked == true) dateTimePickerForm.Value = dtTemp;
            else
                dateTimePickerTo.Value = (DateTime)_schtransactionEntity.EndDateTime;
        }
        
        private void contentHeaderUserControl_BackButtonOnClick(object sender, EventArgs e)
        {
            _changeContentMainFormAction(ContentItemEnum.Transaction);
        }

        private void ManageTransactionUserControl_Load(object sender, EventArgs e)
        {
            _transactionParties = new BindingList<TransactionPartyBinder>();

            foreach (TransactionPartyEntity tranParty in _applicationService.TransactionParties)
            {
                if (tranParty.IsActive)
                {
                    _transactionParties.Add(new TransactionPartyBinder(tranParty));
                }
            }

            transactionPartyComboBox.DataSource = _transactionParties;
            transactionPartyComboBox.DisplayMember = "DisplayValue";
            transactionPartyComboBox.ValueMember = "Id";

            if (_transactionEntity!=null)
            SetTransactionEntity(_transactionEntity);
            if(_schtransactionEntity!=null)
            SetScheduleTransactionEntity(_schtransactionEntity);

            int Consider_Val = 0;
            if (_transactionEntity != null) Consider_Val = _transactionEntity.Id;
            else if (_schtransactionEntity != null) Consider_Val = _schtransactionEntity.Id;

                if (Consider_Val == 0)
                {
                    actionsUserControl.DeleteButtonVisible = false;
                    actionsUserControl.ResetButtonVisible = true;
                    contentHeaderUserControl.MainTitle = "Add Transaction";
                }
                else
                {
                    actionsUserControl.DeleteButtonVisible = true;
                    actionsUserControl.ResetButtonVisible = false;
                    contentHeaderUserControl.MainTitle = "Update Transaction";
                }
        }

        private bool IsFormDataValid()
        {
            bool isValid = true;
            if (_transactionEntity != null) {
                int id = _transactionEntity.Id; }
                 else if (_schtransactionEntity != null){    
                 int id = _schtransactionEntity.Id; }

            string remarks = remarksTextBox.Text;
            string amount = amountNumericUpDown.Text;
            double amountDouble;
            int transactionParty = transactionPartyComboBox.SelectedIndex;

            if (string.IsNullOrWhiteSpace(remarks))
            {
                isValid = false;
                remarksErrorLabel.Text = "Remark is required";
            }
            else
            {
                remarksErrorLabel.Text = "";
            }

            if (string.IsNullOrWhiteSpace(amount))
            {
                isValid = false;
                amountErrorLabel.Text = "Amount is required";
            }
            else if (!double.TryParse(amount, out amountDouble))
            {
                isValid = false;
                amountErrorLabel.Text = "Amount need to be numeric";
            }
            else if (amountDouble <= 0)
            {
                isValid = false;
                amountErrorLabel.Text = "Amount should greater than zero";
            }
            else
            {
                amountErrorLabel.Text = "";
            }

            if (transactionParty < 0)
            {
                isValid = false;
                transactionPartErrorLabel.Text = "Transaction Party is required";
            }
            else
            {
                transactionPartErrorLabel.Text = "";
            }

            if (comboBoxType.SelectedIndex == 1) { 
            if (checkBoxNever.Checked==false)
            {
                DateTime dtCheck = DateTime.Now;
                string SelectedType = this.comboBoxRepeat.GetItemText(this.comboBoxRepeat.SelectedItem).Trim();
                if (SelectedType == ContentRepeatItemEnum.Daily.ToString())
                    dtCheck = dateTimePickerForm.Value.AddDays(1);
                else if (SelectedType == ContentRepeatItemEnum.Weekly.ToString())
                    dtCheck = dateTimePickerForm.Value.AddDays(7);
                else if (SelectedType == ContentRepeatItemEnum.Monthly.ToString())
                    dtCheck = dateTimePickerForm.Value.AddDays(30);
                else
                    dtCheck = dateTimePickerForm.Value.AddYears(1);
                if(dtCheck > dateTimePickerTo.Value)
                {
                    isValid = false;
                    labelInvalidDate.Text = "Enter valid End Date";
                }
                else
                {
                    labelInvalidDate.Text = "";
                }
            }
           }

            return isValid;
        }

        private async void actionsUserControl_SaveButtonOnClick(object sender, EventArgs e)
        {
            if (IsFormDataValid())
            {
                tableLayoutPanel.Enabled = false;
                actionsUserControl.IsEnabledButtons = false;
                int id = 0;

                if (_transactionEntity != null)
                {
                     id = _transactionEntity.Id;
                }
                else if (_schtransactionEntity != null)
                {
                     id = _schtransactionEntity.Id;
                }
                string remarks = remarksTextBox.Text;
                int transactionId =comboBoxType.SelectedIndex;
                double amount = double.Parse(amountNumericUpDown.Text);
                TransactionPartyBinder transactionParty = transactionPartyComboBox.SelectedItem as TransactionPartyBinder;

                if (transactionId==0) {

                    //single time transactions
                    TransactionEntity transactionEntity = new TransactionEntity()
                    {
                        Id = id,
                        Amount = amount,
                        IsIncome = incomeCheckBox.Checked,
                        Remarks = remarks,
                        TransactionPartyId = transactionParty.Id,
                        IsUserPerformed = true,
                        ScheduledTransactionId = null,
                        TransactionDateTime = dateTimePickerOneTimeTransactionDate.Value
                    };

                    if (id == 0)
                    {
                        await _applicationService.InsertTransactionAsync(transactionEntity, true);
                    }
                    else
                    {
                        await _applicationService.UpdateTransactionAsync(transactionEntity);
                    }

                }

                else
                {
                    //Scheduled transactions

                    DateTime dtNextDate = DateTime.Now;
                    string SelectedType = this.comboBoxRepeat.GetItemText(this.comboBoxRepeat.SelectedItem).Trim();
                   
                    if (SelectedType== ContentRepeatItemEnum.Daily.ToString())
                        dtNextDate = dateTimePickerForm.Value.AddDays(1);
                    else if(SelectedType == ContentRepeatItemEnum.Weekly.ToString())
                        dtNextDate = dateTimePickerForm.Value.AddDays(7);
                    else if (SelectedType == ContentRepeatItemEnum.Monthly.ToString())
                        dtNextDate = dateTimePickerForm.Value.AddDays(30);
                    else
                        dtNextDate = dateTimePickerForm.Value.AddYears(1);

                    SheduledTransactionList transactionSheduledEntity = new SheduledTransactionList()
                    {
                        Id = id,
                        TransactionPartyId = transactionParty.Id,
                        Amount = amount,
                        RepeatType = SelectedType,
                        StartDateTime = dateTimePickerForm.Value,
                        EndDateTime= dateTimePickerTo.Value,
                        NextTransactionDate= dtNextDate,
                        InfiniteSchedule =checkBoxNever.Checked,
                        CreatedDateTime =DateTime.Now,
                        IsIncome = incomeCheckBox.Checked,
                        Remarks = remarks,                
                        IsDelete = false,
                        IsActive = checkBoxActive.Checked,

                    };

                    if (id == 0)
                    {
                        await _applicationService.InsertSheduledTransactionListAsync(transactionSheduledEntity);
                    }
                    else
                    {
                        await _applicationService.UpdateSheduledTransactionListAsync(transactionSheduledEntity);
                    }
                }

           
                _changeContentMainFormAction(ContentItemEnum.Transaction);
            }
        }

        private async void actionsUserControl_DeleteButtonOnClick(object sender, EventArgs e)
        {
            int id = 0;

            if (_transactionEntity != null)
            {
                 id = _transactionEntity.Id;
            }
            else if (_schtransactionEntity != null)
            {
                 id = _schtransactionEntity.Id;
            }

            if (id > 0)
            {
                tableLayoutPanel.Enabled = false;
                actionsUserControl.IsEnabledButtons = false;

                DialogResult dr = MessageBox.Show("Are you sure you want to delete this transaction?", "Confrimation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    if (_transactionEntity != null)
                    {
                        await _applicationService.DeleteTransactionAsync(_transactionEntity.Id);
                    }
                    else if (_schtransactionEntity != null)
                    {
                        await _applicationService.DeleteSheduledTransactionAsync(_schtransactionEntity.Id);
                    }
                
                    _changeContentMainFormAction(ContentItemEnum.Transaction);
                }

                tableLayoutPanel.Enabled = true;
                actionsUserControl.IsEnabledButtons = true;
            }
        }

        private void actionsUserControl_ResetButtonOnClick(object sender, EventArgs e)
        {
                SetTransactionEntity();
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxType.SelectedIndex == 1) {
                panel6.Visible = true;
                panel7.Visible = true;
                panel8.Visible = true;
                panel9.Visible = true;
                panel11.Visible = false;
            }
            else
            {
                panel6.Visible = false;
                panel7.Visible = false;
                panel8.Visible = false;
                panel9.Visible = false;
                comboBoxRepeat.SelectedIndex = 2;
                dateTimePickerTo.MinDate = DateTime.Today.AddDays(1);
                dateTimePickerForm.MinDate = DateTime.Today;
                checkBoxNever.Checked = false;
                checkBoxNever.Enabled = true;
                panel11.Visible = true;
               
            }
        }

        private void comboBoxRepeat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxRepeat.SelectedIndex == 4)
            {
                checkBoxNever.Enabled = false;
            }
            else
            {
                checkBoxNever.Enabled = true;
            }
        }
    }
    class TransactionPartyBinder
    {
        public TransactionPartyBinder()
        { }

        public TransactionPartyBinder(TransactionPartyEntity transactionPartyEntity)
        {
            Id = transactionPartyEntity.Id;
            DisplayValue = $"{transactionPartyEntity.Code} - {transactionPartyEntity.Description}";
        }

        public int Id { get; set; }
        public string DisplayValue { get; set; }
    }
}

