namespace Domain.Entities;

/// <summary>
/// A user who is a user of ICMC.
/// </summary>
public class User
{
  /// <summary>
  /// The ID of the user.
  /// </summary>
  public int Id { get; set; }
  /// <summary>
  /// Gets or sets the College ID (CID) of the user.
  /// </summary>
  public string? Cid { get; set; }
  /// <summary>
  /// Gets or sets the imperial email address of the user.
  /// </summary>
  public string? Email { get; set; }
  /// <summary>
  /// The end date of the user.
  /// </summary>
  public string? EndDate { get; set; }
  /// <summary>
  /// Gets or sets the full name of the user.
  /// </summary>
  public string? FullName { get; set; }
  /// <summary>
  /// Gets or sets a value indicating if a user is an admin.
  /// </summary>
  public bool? IsAdmin { get; set; }
  /// <summary>
  /// The login of the user.
  /// </summary>
  public string? Login { get; set; }
  /// <summary>
  /// Gets or sets the member type of a user.
  /// </summary>
  public MemberType? MemberType { get; set; }
  /// <summary>
  /// The order number of the user.
  /// </summary>
  public int? OrderNo { get; set; }
  /// <summary>
  /// The phone number of the user.
  /// </summary>
  public string? PhoneNo { get; set; }
  /// <summary>
  /// The post name of the user.
  /// </summary>
  public string? PostName { get; set; }
  /// <summary>
  /// The start date of the user.
  /// </summary>
  public string? StartDate { get; set; }
  /// <summary>
  /// The one-time password of the user.
  /// </summary>
  public string? Otp { get; set; }
}