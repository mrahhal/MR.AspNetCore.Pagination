﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MR.AspNetCore.Pagination;

/// <summary>
/// Includes reflection helpers used to detect if a method returns a pagination result,
/// unwrapping things like Tasks and AspNetCore's types such as ActionResult.
/// </summary>
public static class PaginationActionDetector
{
	/// <summary>
	/// Determines whether the method returns any pagination result.
	/// Unwraps Tasks and ActionResults.
	/// </summary>
	/// <param name="methodInfo">The method to inspect.</param>
	/// <param name="type">The <see cref="Type"/> of the pagination result's data.</param>
	public static bool IsPaginationResultAction(MethodInfo methodInfo, [NotNullWhen(true)] out Type? type)
	{
		return IsKeysetPaginationResultAction(methodInfo, out type)
			|| IsOffsetPaginationResultAction(methodInfo, out type);
	}

	/// <summary>
	/// Determines whether the method returns a keyset pagination result.
	/// Unwraps Tasks and ActionResults.
	/// </summary>
	/// <param name="methodInfo">The method to inspect.</param>
	/// <param name="type">The <see cref="Type"/> of the pagination result's data.</param>
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
	/// <param name="methodInfo">The method to inspect.</param>
	/// <param name="type">The <see cref="Type"/> of the pagination result's data.</param>
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

		// Unwrap Tasks
		if (targetType.GetGenericTypeDefinition() == typeof(Task<>))
		{
			targetType = targetType.GetGenericArguments()[0];
		}
		if (!targetType.IsGenericType) return false;

		// Unwrap ActionResults
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
