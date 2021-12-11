using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MR.AspNetCore.Pagination;

/// <summary>
/// Includes reflection helpers to detect methods that return pagination results.
/// </summary>
public static class PaginationActionDetector
{
	/// <summary>
	/// Determines whether the method returns a keyset pagination result.
	/// Unwraps Tasks and ActionResults.
	/// </summary>
	public static bool IsKeysetPaginationResultAction(MethodInfo methodInfo, [NotNullWhen(true)] out Type? type)
	{
		return IsPaginationResultAction(
			methodInfo,
			typeof(KeysetPaginationResult<>),
			out type);
	}

	/// <summary>
	/// Determines whether the method returns an offset pagination result.
	/// Unwraps Tasks and ActionResults.
	/// </summary>
	public static bool IsOffsetPaginationResultAction(MethodInfo methodInfo, [NotNullWhen(true)] out Type? type)
	{
		return IsPaginationResultAction(
			methodInfo,
			typeof(OffsetPaginationResult<>),
			out type);
	}

	private static bool IsPaginationResultAction(
		MethodInfo methodInfo,
		Type paginationResultType,
		[NotNullWhen(true)] out Type? type)
	{
		type = null;

		var targetType = methodInfo.ReturnType;
		if (!targetType.IsGenericType) return false;

		if (targetType.GetGenericTypeDefinition() == typeof(Task<>))
		{
			targetType = targetType.GetGenericArguments()[0];
		}

		if (!targetType.IsGenericType) return false;

		if (targetType.GetGenericTypeDefinition().Name == "ActionResult`1")
		{
			targetType = targetType.GetGenericArguments()[0];
		}

		if (!targetType.IsGenericType) return false;

		if (targetType.GetGenericTypeDefinition() == paginationResultType)
		{
			type = targetType.GetGenericArguments()[0];
			return true;
		}

		return false;
	}
}
