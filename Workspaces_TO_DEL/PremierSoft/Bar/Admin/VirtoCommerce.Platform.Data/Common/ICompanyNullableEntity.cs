using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtoCommerce.Platform.Data.Common
{
    /// <summary>
    /// ������� ���� ������, ������ � �������� ������������ ������ �� ������������ �����������
    /// </summary>
    public interface ICompanyNullableEntity
    {
        int? CompanyId { get; set; }
    }
}