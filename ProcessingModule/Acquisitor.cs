using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for periodic polling.
    /// </summary>
    public class Acquisitor : IDisposable
    {
        private AutoResetEvent acquisitionTrigger;
        private IProcessingManager processingManager;
        private Thread acquisitionWorker;
        private IStateUpdater stateUpdater;
        private IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Acquisitor"/> class.
        /// </summary>
        /// <param name="acquisitionTrigger">The acquisition trigger.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="stateUpdater">The state updater.</param>
        /// <param name="configuration">The configuration.</param>
		public Acquisitor(AutoResetEvent acquisitionTrigger, IProcessingManager processingManager, IStateUpdater stateUpdater, IConfiguration configuration)
        {
            this.stateUpdater = stateUpdater;
            this.acquisitionTrigger = acquisitionTrigger;
            this.processingManager = processingManager;
            this.configuration = configuration;
            this.InitializeAcquisitionThread();
            this.StartAcquisitionThread();
        }

        #region Private Methods

        /// <summary>
        /// Initializes the acquisition thread.
        /// </summary>
        private void InitializeAcquisitionThread()
        {
            this.acquisitionWorker = new Thread(Acquisition_DoWork);
            this.acquisitionWorker.Name = "Acquisition thread";
        }

        /// <summary>
        /// Starts the acquisition thread.
        /// </summary>
		private void StartAcquisitionThread()
        {
            acquisitionWorker.Start();
        }

        /// <summary>
        /// Acquisitor thread logic.
        /// </summary>
		private void Acquisition_DoWork()
        {
            //TO DO: IMPLEMENT
            /*while (true)
            {
                try
                {
                    // Čekaj signal za akviziciju
                    acquisitionTrigger.WaitOne();

                    var items = this.configuration.GetConfigurationItems();

                    foreach (var item in items)
                    {
                        if (item.SecondsPassedSinceLastPoll== item.AcquisitionInterval)
                        {
                            // Poziv komande za očitavanje
                            processingManager.ExecuteReadCommand(
                                item,
                                this.configuration.GetTransactionId(),
                                this.configuration.UnitAddress,
                                item.StartAddress,
                                item.NumberOfRegisters
                            );

                            item.SecondsPassedSinceLastPoll = 0; // resetuj brojač
                        }
                        else
                        {
                            item.SecondsPassedSinceLastPoll++; // povećaj brojač
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    break; // zaustavi thread
                }
                catch (Exception ex)
                {
                    // stateUpdater.UpdateStatus($"Error in acquisition: {ex.Message}");
                }

            }*/
            List<IConfigItem> configListItems = configuration.GetConfigurationItems();
            while (true)
            {
                acquisitionTrigger.WaitOne();
                foreach (var i in configListItems)
                {
                    i.SecondsPassedSinceLastPoll += 1;
                    if (i.SecondsPassedSinceLastPoll == i.AcquisitionInterval)
                    {
                        processingManager.ExecuteReadCommand(i, this.configuration.GetTransactionId(), this.configuration.UnitAddress, i.StartAddress, i.NumberOfRegisters);
                        i.SecondsPassedSinceLastPoll = 0;
                    }
                }
            }
        }

        #endregion Private Methods

        /// <inheritdoc />
        public void Dispose()
        {
            acquisitionWorker.Abort();
        }
    }
}