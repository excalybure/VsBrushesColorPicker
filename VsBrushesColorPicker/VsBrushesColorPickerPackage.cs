using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace VsBrushesColorPicker
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(VsBrushesColorPickerPackage.PackageGuidString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideToolWindow(typeof(ColorListToolWindow))]
    public sealed class VsBrushesColorPickerPackage : AsyncPackage
    {
        /// <summary>
        /// VsBrushesColorPickerPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "0d8add0f-d9d9-40e9-968a-537edefb3137";

        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("5e9b6f9d-5d5c-4b0b-9e5c-1e2f8d5a9b11");

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var cmdId = new CommandID(CommandSet, CommandId);
                var menuItem = new OleMenuCommand(ExecuteShowToolWindow, cmdId);
                commandService.AddCommand(menuItem);
            }
        }

        private void ExecuteShowToolWindow(object sender, EventArgs e)
        {
            JoinableTaskFactory.RunAsync(async () =>
            {
                await ShowColorListToolWindowAsync();
            }).FileAndForget("VsBrushesColorPicker/ShowToolWindow");
        }

        private async Task ShowColorListToolWindowAsync()
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            ToolWindowPane window = await ShowToolWindowAsync(typeof(ColorListToolWindow), 0, true, DisposalToken);
            if (window?.Frame is IVsWindowFrame windowFrame)
            {
                ErrorHandler.ThrowOnFailure(windowFrame.Show());
            }
        }

        #endregion
    }
}
