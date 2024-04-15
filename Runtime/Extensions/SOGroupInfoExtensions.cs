namespace F2F
{
	public static class SOGroupInfoExtensions
	{
		internal static bool TryGetType(this SOGroupInfo groupInfo, out System.Type groupType)
		{
			if (groupInfo.Default != null)
			{
				groupType = groupInfo.Default.GetType();
				return true;
			}

			foreach (var soInfo in groupInfo.Elements)
			{
				if (soInfo.SO != null)
				{
					groupType = soInfo.SO.GetType();
					return true;
				}
			}

			groupType = null;
			return false;
		}

		internal static bool HasNullReferences(this SOGroupInfo groupInfo)
		{
			if (groupInfo.Default == null)
			{
				return true;
			}

			var elements = groupInfo.Elements;
			var nElements = groupInfo.Elements.Count;
			for (var iElement = 0; iElement < nElements; iElement++)
			{
				var elementInfo = elements[iElement];
				if (elementInfo.SO == null)
				{
					return true;
				}
			}

			return false;
		}
	}
}