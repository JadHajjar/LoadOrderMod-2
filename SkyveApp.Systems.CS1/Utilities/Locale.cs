﻿using Extensions;

using SkyveApp.Domain.Systems;

namespace SkyveApp.Systems.CS1.Utilities;
public class Locale : LocaleHelper, ILocale
{
	private static readonly Locale _instance = new();

	public static void Load() { _ = _instance; }

	public Translation Get(string key)
	{
		return GetGlobalText(key);
	}

	public Locale() : base($"{nameof(SkyveApp)}.Properties.Locale.json") { }

	public static Translation Dashboard => _instance.GetText(nameof(Dashboard));
	public static Translation StartCities => _instance.GetText(nameof(StartCities));
	public static Translation StopCities => _instance.GetText(nameof(StopCities));
	public static Translation ProfileBubble => _instance.GetText(nameof(ProfileBubble));
	public static Translation AssetsBubble => _instance.GetText(nameof(AssetsBubble));
	public static Translation ModsBubble => _instance.GetText(nameof(ModsBubble));
	public static Translation Local => _instance.GetText(nameof(Local));
	public static Translation Workshop => _instance.GetText(nameof(Workshop));
	public static Translation Enabled => _instance.GetText(nameof(Enabled));
	public static Translation Disabled => _instance.GetText(nameof(Disabled));
	public static Translation Included => _instance.GetText(nameof(Included));
	public static Translation Excluded => _instance.GetText(nameof(Excluded));
	public static Translation Loading => _instance.GetText(nameof(Loading));
	public static Translation Server => _instance.GetText(nameof(Server));
	public static Translation Vanilla => _instance.GetText(nameof(Vanilla));
	public static Translation UpToDate => _instance.GetText(nameof(UpToDate));
	public static Translation StatusUnknown => _instance.GetText(nameof(StatusUnknown));
	public static Translation OutOfDate => _instance.GetText(nameof(OutOfDate));
	public static Translation PartiallyDownloaded => _instance.GetText(nameof(PartiallyDownloaded));
	public static Translation CompatibilityReport => _instance.GetText(nameof(CompatibilityReport));
	public static Translation AutoProfileSaveOn => _instance.GetText(nameof(AutoProfileSaveOn));
	public static Translation AutoProfileSaveOff => _instance.GetText(nameof(AutoProfileSaveOff));
	public static Translation TemporaryProfileCanNotBeEdited => _instance.GetText(nameof(TemporaryProfileCanNotBeEdited));
	public static Translation LaunchSettings => _instance.GetText(nameof(LaunchSettings));
	public static Translation IncludesItemsYouDoNotHave => _instance.GetText(nameof(IncludesItemsYouDoNotHave));
	public static Translation ContentAndInfo => _instance.GetText(nameof(ContentAndInfo));
	public static Translation OtherProfiles => _instance.GetText(nameof(OtherProfiles));
	public static Translation Filters => _instance.GetText(nameof(Filters));
	public static Translation TotalSize => _instance.GetText(nameof(TotalSize));
	public static Translation AssetStatus => _instance.GetText(nameof(AssetStatus));
	public static Translation ModStatus => _instance.GetText(nameof(ModStatus));
	public static Translation CompatibilityStatus => _instance.GetText(nameof(CompatibilityStatus));
	public static Translation AnyStatus => _instance.GetText(nameof(AnyStatus));
	public static Translation AnyCompatibilityStatus => _instance.GetText(nameof(AnyCompatibilityStatus));
	public static Translation Subscribe => _instance.GetText(nameof(Subscribe));
	public static Translation Switch => _instance.GetText(nameof(Switch));
	public static Translation Enable => _instance.GetText(nameof(Enable));
	public static Translation ModOwned => _instance.GetText(nameof(ModOwned));
	public static Translation Settings => _instance.GetText(nameof(Settings));
	public static Translation NoLocalPackagesFound => _instance.GetText(nameof(NoLocalPackagesFound));
	public static Translation NoPackagesMatchFilters => _instance.GetText(nameof(NoPackagesMatchFilters));
	public static Translation Actions => _instance.GetText(nameof(Actions));
	public static Translation Utilities => _instance.GetText(nameof(Utilities));
	public static Translation CollectionTitle => _instance.GetText(nameof(CollectionTitle));
	public static Translation Options => _instance.GetText(nameof(Options));
	public static Translation CrNotAvailable => _instance.GetText(nameof(CrNotAvailable));
	public static Translation ModsWithMinorIssues => _instance.GetText(nameof(ModsWithMinorIssues));
	public static Translation ModsWithMajorIssues => _instance.GetText(nameof(ModsWithMajorIssues));
	public static Translation ModsShouldUnsub => _instance.GetText(nameof(ModsShouldUnsub));
	public static Translation ModsNoIssues => _instance.GetText(nameof(ModsNoIssues));
	public static Translation Preferences => _instance.GetText(nameof(Preferences));
	public static Translation StartScratch => _instance.GetText(nameof(StartScratch));
	public static Translation ContinueFromCurrent => _instance.GetText(nameof(ContinueFromCurrent));
	public static Translation ProfileUsage => _instance.GetText(nameof(ProfileUsage));
	public static Translation CheckFolderInOptions => _instance.GetText(nameof(CheckFolderInOptions));
	public static Translation SomePackagesWillBeDisabled => _instance.GetText(nameof(SomePackagesWillBeDisabled));
	public static Translation AffectedPackagesAre => _instance.GetText(nameof(AffectedPackagesAre));
	public static Translation ConfirmDeleteProfile => _instance.GetText(nameof(ConfirmDeleteProfile));
	public static Translation ProfileReplace => _instance.GetText(nameof(ProfileReplace));
	public static Translation ProfileExclude => _instance.GetText(nameof(ProfileExclude));
	public static Translation ProfileMerge => _instance.GetText(nameof(ProfileMerge));
	public static Translation ProfileDelete => _instance.GetText(nameof(ProfileDelete));
	public static Translation ShouldNotBeSubscribed => _instance.GetText(nameof(ShouldNotBeSubscribed));
	public static Translation LoadingScreenMod => _instance.GetText(nameof(LoadingScreenMod));
	public static Translation ExcludeInclude => _instance.GetText(nameof(ExcludeInclude));
	public static Translation EnableDisable => _instance.GetText(nameof(EnableDisable));
	public static Translation OpenPackagePage => _instance.GetText(nameof(OpenPackagePage));
	public static Translation OpenLocalFolder => _instance.GetText(nameof(OpenLocalFolder));
	public static Translation ViewOnSteam => _instance.GetText(nameof(ViewOnSteam));
	public static Translation ReDownloadPackage => _instance.GetText(nameof(ReDownloadPackage));
	public static Translation DownloadPackage => _instance.GetText(nameof(DownloadPackage));
	public static Translation CopySteamId => _instance.GetText(nameof(CopySteamId));
	public static Translation OpenAuthorPage => _instance.GetText(nameof(OpenAuthorPage));
	public static Translation Sorting => _instance.GetText(nameof(Sorting));
	public static Translation FolderSettings => _instance.GetText(nameof(FolderSettings));
	public static Translation ChangingFoldersRequiresRestart => _instance.GetText(nameof(ChangingFoldersRequiresRestart));
	public static Translation CreateProfileHere => _instance.GetText(nameof(CreateProfileHere));
	public static Translation TemporaryPlayset => _instance.GetText(nameof(TemporaryPlayset));
	public static Translation CouldNotCreateProfile => _instance.GetText(nameof(CouldNotCreateProfile));
	public static Translation ProfileNameChangedIllegalChars => _instance.GetText(nameof(ProfileNameChangedIllegalChars));
	public static Translation ProfileSaveInfo => _instance.GetText(nameof(ProfileSaveInfo));
	public static Translation From => _instance.GetText(nameof(From));
	public static Translation To => _instance.GetText(nameof(To));
	public static Translation DateSubscribed => _instance.GetText(nameof(DateSubscribed));
	public static Translation DateUpdated => _instance.GetText(nameof(DateUpdated));
	public static Translation PackageStatus => _instance.GetText(nameof(PackageStatus));
	public static Translation MultiplePackagesIncluded => _instance.GetText(nameof(MultiplePackagesIncluded));
	public static Translation CopyWorkshopLink => _instance.GetText(nameof(CopyWorkshopLink));
	public static Translation DeletePackage => _instance.GetText(nameof(DeletePackage));
	public static Translation UnsubscribePackage => _instance.GetText(nameof(UnsubscribePackage));
	public static Translation CopyWorkshopId => _instance.GetText(nameof(CopyWorkshopId));
	public static Translation CopyAuthorLink => _instance.GetText(nameof(CopyAuthorLink));
	public static Translation CopyAuthorId => _instance.GetText(nameof(CopyAuthorId));
	public static Translation Copy => _instance.GetText(nameof(Copy));
	public static Translation YourProfiles => _instance.GetText(nameof(YourProfiles));
	public static Translation UnFavoriteThisProfile => _instance.GetText(nameof(UnFavoriteThisProfile));
	public static Translation FavoriteThisProfile => _instance.GetText(nameof(FavoriteThisProfile));
	public static Translation ProfileCreationFailed => _instance.GetText(nameof(ProfileCreationFailed));
	public static Translation Tags => _instance.GetText(nameof(Tags));
	public static Translation DLCs => _instance.GetText(nameof(DLCs));
	public static Translation NoDlcsNoInternet => _instance.GetText(nameof(NoDlcsNoInternet));
	public static Translation NoDlcsOpenGame => _instance.GetText(nameof(NoDlcsOpenGame));
	public static Translation DlcUpdateNotice => _instance.GetText(nameof(DlcUpdateNotice));
	public static Translation DlcCount => _instance.GetText(nameof(DlcCount));
	public static Translation IncludeAllItemsInThisPackage => _instance.GetText(nameof(IncludeAllItemsInThisPackage));
	public static Translation ExcludeAllItemsInThisPackage => _instance.GetText(nameof(ExcludeAllItemsInThisPackage));
	public static Translation Unfiltered => _instance.GetText(nameof(Unfiltered));
	public static Translation AnyTags => _instance.GetText(nameof(AnyTags));
	public static Translation MovePackageToLocalFolder => _instance.GetText(nameof(MovePackageToLocalFolder));
	public static Translation TotalItems => _instance.GetText(nameof(TotalItems));
	public static Translation MissingItemsRemain => _instance.GetText(nameof(MissingItemsRemain));
	public static Translation ModIsPrivate => _instance.GetText(nameof(ModIsPrivate));
	public static Translation SelectedFileInvalid => _instance.GetText(nameof(SelectedFileInvalid));
	public static Translation NoItemsToBeDisplayed => _instance.GetText(nameof(NoItemsToBeDisplayed));
	public static Translation FirstSetupInfo => _instance.GetText(nameof(FirstSetupInfo));
	public static Translation SetupIncomplete => _instance.GetText(nameof(SetupIncomplete));
	public static Translation CopyAuthorSteamId => _instance.GetText(nameof(CopyAuthorSteamId));
	public static Translation LoadProfile => _instance.GetText(nameof(LoadProfile));
	public static Translation CloseCitiesToSub => _instance.GetText(nameof(CloseCitiesToSub));
	public static Translation ShowingFilteredItems => _instance.GetText(nameof(ShowingFilteredItems));
	public static Translation ShowingCount => _instance.GetText(nameof(ShowingCount));
	public static Translation ShowingCountWarning => _instance.GetText(nameof(ShowingCountWarning));
	public static Translation ItemsHidden => _instance.GetText(nameof(ItemsHidden));
	public static Translation ClearFoldersPromptTitle => _instance.GetText(nameof(ClearFoldersPromptTitle));
	public static Translation ClearFoldersPrompt => _instance.GetText(nameof(ClearFoldersPrompt));
	public static Translation LaunchTooltip => _instance.GetText(nameof(LaunchTooltip));
	public static Translation AreYouSure => _instance.GetText(nameof(AreYouSure));
	public static Translation AssetOutOfDate => _instance.GetText(nameof(AssetOutOfDate));
	public static Translation AssetOutOfDatePlural => _instance.GetText(nameof(AssetOutOfDatePlural));
	public static Translation CopyVersionNumber => _instance.GetText(nameof(CopyVersionNumber));
	public static Translation SubscribeToItem => _instance.GetText(nameof(SubscribeToItem));
	public static Translation MultipleLOM => _instance.GetText(nameof(MultipleLOM));
	public static Translation FilterByThisReportStatus => _instance.GetText(nameof(FilterByThisReportStatus));
	public static Translation FilterByThisPackageStatus => _instance.GetText(nameof(FilterByThisPackageStatus));
	public static Translation FilterSinceThisDate => _instance.GetText(nameof(FilterSinceThisDate));
	public static Translation FilterByThisTag => _instance.GetText(nameof(FilterByThisTag));
	public static Translation ItemsShouldNotBeSubscribedInfo => _instance.GetText(nameof(ItemsShouldNotBeSubscribedInfo));
	public static Translation WouldYouLikeToSkipThose => _instance.GetText(nameof(WouldYouLikeToSkipThose));
	public static Translation LOTWillRestart => _instance.GetText(nameof(LOTWillRestart));
	public static Translation UpdatingLot => _instance.GetText(nameof(UpdatingLot));
	public static Translation SubscribingRequiresGameToOpenTitle => _instance.GetText(nameof(SubscribingRequiresGameToOpenTitle));
	public static Translation SubscribingRequiresGameToOpen => _instance.GetText(nameof(SubscribingRequiresGameToOpen));
	public static Translation HelpLogs => _instance.GetText(nameof(HelpLogs));
	public static Translation DisableFpsBoosterDebug => _instance.GetText(nameof(DisableFpsBoosterDebug));
	public static Translation DefaultLogViewInfo => _instance.GetText(nameof(DefaultLogViewInfo));
	public static Translation ModIncludedTotal => _instance.GetText(nameof(ModIncludedTotal));
	public static Translation ModIncludedEnabledTotal => _instance.GetText(nameof(ModIncludedEnabledTotal));
	public static Translation ModIncludedAndEnabledTotal => _instance.GetText(nameof(ModIncludedAndEnabledTotal));
	public static Translation PackageIncludedTotal => _instance.GetText(nameof(PackageIncludedTotal));
	public static Translation PackageIncludedEnabledTotal => _instance.GetText(nameof(PackageIncludedEnabledTotal));
	public static Translation PackageIncludedAndEnabledTotal => _instance.GetText(nameof(PackageIncludedAndEnabledTotal));
	public static Translation AssetIncludedTotal => _instance.GetText(nameof(AssetIncludedTotal));
	public static Translation ClickAuthorFilter => _instance.GetText(nameof(ClickAuthorFilter));
	public static Translation ActionUnreversible => _instance.GetText(nameof(ActionUnreversible));
	public static Translation FavoriteTotal => _instance.GetText(nameof(FavoriteTotal));
	public static Translation FavoriteProfileTotal => _instance.GetText(nameof(FavoriteProfileTotal));
	public static Translation ProfileFilter => _instance.GetText(nameof(ProfileFilter));
	public static Translation CheckDocumentsFolder => _instance.GetText(nameof(CheckDocumentsFolder));
	public static Translation FailedToSaveLanguage => _instance.GetText(nameof(FailedToSaveLanguage));
	public static Translation FailedToOpenTC => _instance.GetText(nameof(FailedToOpenTC));
	public static Translation FailedToDeleteItem => _instance.GetText(nameof(FailedToDeleteItem));
	public static Translation DeleteAsset => _instance.GetText(nameof(DeleteAsset));
	public static Translation IncludeThisItemInAllProfiles => _instance.GetText(nameof(IncludeThisItemInAllProfiles));
	public static Translation ExcludeThisItemInAllProfiles => _instance.GetText(nameof(ExcludeThisItemInAllProfiles));
	public static Translation CopyPackageName => _instance.GetText(nameof(CopyPackageName));
	public static Translation CopyAuthorName => _instance.GetText(nameof(CopyAuthorName));
	public static Translation CopyFolderName => _instance.GetText(nameof(CopyFolderName));
	public static Translation ProfileNameUsed => _instance.GetText(nameof(ProfileNameUsed));
	public static Translation FailedToImportLegacyProfile => _instance.GetText(nameof(FailedToImportLegacyProfile));
	public static Translation CurrentPlayset => _instance.GetText(nameof(CurrentPlayset));
	public static Translation ProfileStillLoading => _instance.GetText(nameof(ProfileStillLoading));
	public static Translation ApplyProfileNameBeforeExit => _instance.GetText(nameof(ApplyProfileNameBeforeExit));
	public static Translation SteamNotOpenTo => _instance.GetText(nameof(SteamNotOpenTo));
	public static Translation OpenSteamToContinue => _instance.GetText(nameof(OpenSteamToContinue));
	public static Translation IncludeExcludeOtherProfile => _instance.GetText(nameof(IncludeExcludeOtherProfile));
	public static Translation ChangeProfileColor => _instance.GetText(nameof(ChangeProfileColor));
	public static Translation OpenProfileFolder => _instance.GetText(nameof(OpenProfileFolder));
	public static Translation CreateShortcutProfile => _instance.GetText(nameof(CreateShortcutProfile));
	public static Translation AskToLaunchGameForShortcut => _instance.GetText(nameof(AskToLaunchGameForShortcut));
	public static Translation NoProfilesFound => _instance.GetText(nameof(NoProfilesFound));
	public static Translation NoProfilesMatchFilters => _instance.GetText(nameof(NoProfilesMatchFilters));
	public static Translation Author => _instance.GetText(nameof(Author));
	public static Translation AnyAuthor => _instance.GetText(nameof(AnyAuthor));
	public static Translation ItemsCount => _instance.GetText(nameof(ItemsCount));
	public static Translation AuthorsSelected => _instance.GetText(nameof(AuthorsSelected));
	public static Translation AnyIssue => _instance.GetText(nameof(AnyIssue));
	public static Translation IncludeAll => _instance.GetText(nameof(IncludeAll));
	public static Translation ExcludeAll => _instance.GetText(nameof(ExcludeAll));
	public static Translation EnableAll => _instance.GetText(nameof(EnableAll));
	public static Translation DisableAll => _instance.GetText(nameof(DisableAll));
	public static Translation UnsubscribeAll => _instance.GetText(nameof(UnsubscribeAll));
	public static Translation CopyAllIds => _instance.GetText(nameof(CopyAllIds));
	public static Translation DeleteAll => _instance.GetText(nameof(DeleteAll));
	public static Translation ControlClickTo => _instance.GetText(nameof(ControlClickTo));
	public static Translation FilterByThisAuthor => _instance.GetText(nameof(FilterByThisAuthor));
	public static Translation AddToSearch => _instance.GetText(nameof(AddToSearch));
	public static Translation CopyToClipboard => _instance.GetText(nameof(CopyToClipboard));
	public static Translation ViewPackageCR => _instance.GetText(nameof(ViewPackageCR));
	public static Translation FilterByThisEnabledStatus => _instance.GetText(nameof(FilterByThisEnabledStatus));
	public static Translation FilterByThisIncludedStatus => _instance.GetText(nameof(FilterByThisIncludedStatus));
	public static Translation CleanupInfo => _instance.GetText(nameof(CleanupInfo));
	public static Translation CloseCitiesToClean => _instance.GetText(nameof(CloseCitiesToClean));
	public static Translation CleanupRequiresGameToOpen => _instance.GetText(nameof(CleanupRequiresGameToOpen));
	public static Translation SubscribersCount => _instance.GetText(nameof(SubscribersCount));
	public static Translation RatingCount => _instance.GetText(nameof(RatingCount));
	public static Translation SubscribeAll => _instance.GetText(nameof(SubscribeAll));
	public static Translation MissingLSMReport => _instance.GetText(nameof(MissingLSMReport));
	public static Translation UnusedLSMReport => _instance.GetText(nameof(UnusedLSMReport));
	public static Translation MissingPackagesProfile => _instance.GetText(nameof(MissingPackagesProfile));
	public static Translation SearchWorkshop => _instance.GetText(nameof(SearchWorkshop));
	public static Translation SearchWorkshopBrowser => _instance.GetText(nameof(SearchWorkshopBrowser));
	public static Translation VotingTitle => _instance.GetText(nameof(VotingTitle));
	public static Translation VotingInfo1 => _instance.GetText(nameof(VotingInfo1));
	public static Translation VotingInfo2 => _instance.GetText(nameof(VotingInfo2));
	public static Translation VotingInfo3 => _instance.GetText(nameof(VotingInfo3));
	public static Translation VotingInfo4 => _instance.GetText(nameof(VotingInfo4));
	public static Translation VotingInfo5 => _instance.GetText(nameof(VotingInfo5));
	public static Translation VotingInfo6 => _instance.GetText(nameof(VotingInfo6));
	public static Translation UnknownPackage => _instance.GetText(nameof(UnknownPackage));
	public static Translation AssetsWithMinorIssues => _instance.GetText(nameof(AssetsWithMinorIssues));
	public static Translation AssetsWithMajorIssues => _instance.GetText(nameof(AssetsWithMajorIssues));
	public static Translation AssetsShouldUnsub => _instance.GetText(nameof(AssetsShouldUnsub));
	public static Translation SelectThisPackage => _instance.GetText(nameof(SelectThisPackage));
	public static Translation ControlToSelectMultiplePackages => _instance.GetText(nameof(ControlToSelectMultiplePackages));
	public static Translation Include => _instance.GetText(nameof(Include));
	public static Translation Snooze => _instance.GetText(nameof(Snooze));
	public static Translation UnSnooze => _instance.GetText(nameof(UnSnooze));
	public static Translation Unsubscribe => _instance.GetText(nameof(Unsubscribe));
	public static Translation Exclude => _instance.GetText(nameof(Exclude));
	public static Translation Mod => _instance.GetText(nameof(Mod));
	public static Translation Asset => _instance.GetText(nameof(Asset));
	public static Translation Package => _instance.GetText(nameof(Package));
	public static Translation Profile => _instance.GetText(nameof(Profile));
	public static Translation IncludedCount => _instance.GetText(nameof(IncludedCount));
	public static Translation EnabledCount => _instance.GetText(nameof(EnabledCount));
	public static Translation IncludedEnabledCount => _instance.GetText(nameof(IncludedEnabledCount));
	public static Translation LoadedCount => _instance.GetText(nameof(LoadedCount));
	public static Translation EditCompatibility => _instance.GetText(nameof(EditCompatibility));
	public static Translation AnyUsage => _instance.GetText(nameof(AnyUsage));
	public static Translation ReviewRequestSent => _instance.GetText(nameof(ReviewRequestSent));
	public static Translation ReviewRequestFailed => _instance.GetText(nameof(ReviewRequestFailed));
	public static Translation EditTags => _instance.GetText(nameof(EditTags));
	public static Translation AddCustomTag => _instance.GetText(nameof(AddCustomTag));
	public static Translation OutOfDateCount => _instance.GetText(nameof(OutOfDateCount));
	public static Translation IncompleteCount => _instance.GetText(nameof(IncompleteCount));
	public static Translation PackageIsIncomplete => _instance.GetText(nameof(PackageIsIncomplete));
	public static Translation PackageIsMaybeOutOfDate => _instance.GetText(nameof(PackageIsMaybeOutOfDate));
	public static Translation PackageIsOutOfDate => _instance.GetText(nameof(PackageIsOutOfDate));
	public static Translation PackageIsNotDownloaded => _instance.GetText(nameof(PackageIsNotDownloaded));
	public static Translation PackageIsUnknown => _instance.GetText(nameof(PackageIsUnknown));
	public static Translation PackageIsRemoved => _instance.GetText(nameof(PackageIsRemoved));
	public static Translation RemovedFromSteam => _instance.GetText(nameof(RemovedFromSteam));
	public static Translation Missing => _instance.GetText(nameof(Missing));
	public static Translation LoggedInAs => _instance.GetText(nameof(LoggedInAs));
	public static Translation SendReview => _instance.GetText(nameof(SendReview));
	public static Translation UnknownUser => _instance.GetText(nameof(UnknownUser));
	public static Translation SelectPackage => _instance.GetText(nameof(SelectPackage));
	public static Translation AddMeaningfulDescription => _instance.GetText(nameof(AddMeaningfulDescription));
	public static Translation CleanupComplete => _instance.GetText(nameof(CleanupComplete));
	public static Translation RedownloadComplete => _instance.GetText(nameof(RedownloadComplete));
	public static Translation PendingSubscribeTo => _instance.GetText(nameof(PendingSubscribeTo));
	public static Translation PendingUnsubscribeFrom => _instance.GetText(nameof(PendingUnsubscribeFrom));
	public static Translation ThisSubscribesTo => _instance.GetText(nameof(ThisSubscribesTo));
	public static Translation ThisUnsubscribesFrom => _instance.GetText(nameof(ThisUnsubscribesFrom));
	public static Translation ViewThisProfilesPackages => _instance.GetText(nameof(ViewThisProfilesPackages));
	public static Translation DownloadAll => _instance.GetText(nameof(DownloadAll));
	public static Translation ReDownloadAll => _instance.GetText(nameof(ReDownloadAll));
	public static Translation PendingDownloads => _instance.GetText(nameof(PendingDownloads));
	public static Translation PendingDeletions => _instance.GetText(nameof(PendingDeletions));
	public static Translation YouHavePackagesUser => _instance.GetText(nameof(YouHavePackagesUser));
	public static Translation ShareProfile => _instance.GetText(nameof(ShareProfile));
	public static Translation FailedToFetchLogs => _instance.GetText(nameof(FailedToFetchLogs));
	public static Translation AllUsages => _instance.GetText(nameof(AllUsages));
	public static Translation Invalid => _instance.GetText(nameof(Invalid));
	public static Translation UpdateProfile => _instance.GetText(nameof(UpdateProfile));
	public static Translation FailedToRetrieveProfiles => _instance.GetText(nameof(FailedToRetrieveProfiles));
	public static Translation DownloadProfile => _instance.GetText(nameof(DownloadProfile));
	public static Translation DiscoverProfiles => _instance.GetText(nameof(DiscoverProfiles));
	public static Translation FailedToDownloadProfile => _instance.GetText(nameof(FailedToDownloadProfile));
	public static Translation UpdateProfileTip => _instance.GetText(nameof(UpdateProfileTip));
	public static Translation DownloadProfileTip => _instance.GetText(nameof(DownloadProfileTip));
	public static Translation EditProfileThumbnail => _instance.GetText(nameof(EditProfileThumbnail));
	public static Translation MakePrivate => _instance.GetText(nameof(MakePrivate));
	public static Translation MakePublic => _instance.GetText(nameof(MakePublic));
	public static Translation CopyProfileLink => _instance.GetText(nameof(CopyProfileLink));
	public static Translation FailedToUploadProfile => _instance.GetText(nameof(FailedToUploadProfile));
	public static Translation FailedToDeleteProfile => _instance.GetText(nameof(FailedToDeleteProfile));
	public static Translation FailedToUpdateProfile => _instance.GetText(nameof(FailedToUpdateProfile));
	public static Translation ImportFromLink => _instance.GetText(nameof(ImportFromLink));
	public static Translation PasteProfileId => _instance.GetText(nameof(PasteProfileId));
}