import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import {
  Table,
  Button,
  Input,
  Space,
  Card,
  Row,
  Col,
  Select,
  Tag,
  Popconfirm,
  message,
  Tooltip,
  Badge,
  Statistic,
  Avatar,
} from 'antd';
import {
  PlusOutlined,
  SearchOutlined,
  EditOutlined,
  DeleteOutlined,
  EyeOutlined,
  ReloadOutlined,
  FilterOutlined,
  ClearOutlined,
  TeamOutlined,
  UserOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { AppDispatch, RootState } from '../../store/store';
import {
  fetchAffiliations,
  deleteAffiliation,
  setFilters,
  clearFilters,
  selectAffiliations,
  selectIsLoading,
  selectError,
  selectTotalCount,
  selectFilters,
  Affiliation,
} from '../../store/slices/affiliationSlice';

const { Search } = Input;
const { Option } = Select;

const AffiliationList: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();
  
  const affiliations = useSelector(selectAffiliations);
  const isLoading = useSelector(selectIsLoading);
  const error = useSelector(selectError);
  const totalCount = useSelector(selectTotalCount);
  const filters = useSelector(selectFilters);

  const [searchValue, setSearchValue] = useState(filters.search);
  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);

  useEffect(() => {
    dispatch(fetchAffiliations());
  }, [dispatch]);

  useEffect(() => {
    if (error) {
      message.error(error);
    }
  }, [error]);

  const handleSearch = (value: string) => {
    setSearchValue(value);
    dispatch(setFilters({ search: value }));
  };

  const handleFilterChange = (key: string, value: string) => {
    dispatch(setFilters({ [key]: value }));
  };

  const handleClearFilters = () => {
    setSearchValue('');
    dispatch(clearFilters());
    dispatch(fetchAffiliations());
  };

  const handleDelete = async (affiliationId: number) => {
    try {
      await dispatch(deleteAffiliation(affiliationId)).unwrap();
      message.success('Affiliation deleted successfully');
    } catch (error) {
      message.error('Failed to delete affiliation');
    }
  };

  const handleRefresh = () => {
    dispatch(fetchAffiliations());
  };

  const columns: ColumnsType<Affiliation> = [
    {
      title: 'Employee',
      key: 'employee',
      width: 200,
      fixed: 'left',
      render: (_, record) => (
        <Space>
          <Avatar 
            size="small" 
            icon={<UserOutlined />}
            src={`https://ui-avatars.com/api/?name=${record.employeeName}&size=32&background=1890ff&color=fff`}
          />
          <div>
            <div style={{ fontWeight: 500 }}>{record.employeeName}</div>
            <div style={{ fontSize: '12px', color: '#666' }}>ID: {record.employeeId}</div>
          </div>
        </Space>
      ),
    },
    {
      title: 'Role Type',
      dataIndex: 'roleType',
      key: 'roleType',
      width: 150,
      render: (roleType) => (
        <Tag color="blue">{roleType}</Tag>
      ),
    },
    {
      title: 'Practice',
      dataIndex: 'practice',
      key: 'practice',
      width: 150,
      render: (practice) => (
        <Tag color="green">{practice}</Tag>
      ),
    },
    {
      title: 'Location Scope',
      dataIndex: 'locationScope',
      key: 'locationScope',
      width: 150,
      render: (locationScope) => locationScope || '-',
    },
    {
      title: 'Effective Date',
      dataIndex: 'effectiveDate',
      key: 'effectiveDate',
      width: 120,
      render: (date) => new Date(date).toLocaleDateString(),
    },
    {
      title: 'Expiration Date',
      dataIndex: 'expirationDate',
      key: 'expirationDate',
      width: 120,
      render: (date) => date ? new Date(date).toLocaleDateString() : '-',
    },
    {
      title: 'Status',
      dataIndex: 'isActive',
      key: 'isActive',
      width: 100,
      render: (isActive) => (
        <Tag color={isActive ? 'success' : 'error'}>
          {isActive ? 'Active' : 'Inactive'}
        </Tag>
      ),
    },
    {
      title: 'Last Modified',
      dataIndex: 'lastModifiedDate',
      key: 'lastModifiedDate',
      width: 120,
      render: (date) => new Date(date).toLocaleDateString(),
    },
    {
      title: 'Actions',
      key: 'actions',
      width: 150,
      fixed: 'right',
      render: (_, record) => (
        <Space size="small">
          <Tooltip title="View Details">
            <Button
              type="text"
              icon={<EyeOutlined />}
              onClick={() => navigate(`/affiliations/${record.affiliationId}`)}
            />
          </Tooltip>
          <Tooltip title="Edit">
            <Button
              type="text"
              icon={<EditOutlined />}
              onClick={() => navigate(`/affiliations/${record.affiliationId}/edit`)}
            />
          </Tooltip>
          <Tooltip title="Delete">
            <Popconfirm
              title="Are you sure you want to delete this affiliation?"
              onConfirm={() => handleDelete(record.affiliationId)}
              okText="Yes"
              cancelText="No"
            >
              <Button type="text" danger icon={<DeleteOutlined />} />
            </Popconfirm>
          </Tooltip>
        </Space>
      ),
    },
  ];

  const rowSelection = {
    selectedRowKeys,
    onChange: (newSelectedRowKeys: React.Key[]) => {
      setSelectedRowKeys(newSelectedRowKeys);
    },
  };

  // Get unique values for filters
  const uniquePractices = Array.from(new Set(affiliations.map(aff => aff.practice)));
  const uniqueRoleTypes = Array.from(new Set(affiliations.map(aff => aff.roleType)));

  return (
    <div style={{ padding: '24px' }}>
      <Card>
        <Row gutter={[16, 16]} align="middle" style={{ marginBottom: 24 }}>
          <Col flex="auto">
            <h2 style={{ margin: 0 }}>Affiliations</h2>
          </Col>
          <Col>
            <Space>
              <Button
                icon={<ReloadOutlined />}
                onClick={handleRefresh}
                loading={isLoading}
              >
                Refresh
              </Button>
              <Button
                type="primary"
                icon={<PlusOutlined />}
                onClick={() => navigate('/affiliations/new')}
              >
                Add Affiliation
              </Button>
            </Space>
          </Col>
        </Row>

        {/* Statistics */}
        <Row gutter={16} style={{ marginBottom: 24 }}>
          <Col span={6}>
            <Card>
              <Statistic
                title="Total Affiliations"
                value={totalCount}
                valueStyle={{ color: '#3f8600' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Active Affiliations"
                value={affiliations.filter(aff => aff.isActive).length}
                valueStyle={{ color: '#1890ff' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Unique Employees"
                value={Array.from(new Set(affiliations.map(aff => aff.employeeId))).length}
                valueStyle={{ color: '#722ed1' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Unique Practices"
                value={uniquePractices.length}
                valueStyle={{ color: '#fa8c16' }}
              />
            </Card>
          </Col>
        </Row>

        {/* Filters */}
        <Card style={{ marginBottom: 16 }}>
          <Row gutter={[16, 16]} align="middle">
            <Col span={8}>
              <Search
                placeholder="Search affiliations..."
                value={searchValue}
                onChange={(e) => setSearchValue(e.target.value)}
                onSearch={handleSearch}
                enterButton={<SearchOutlined />}
                allowClear
              />
            </Col>
            <Col span={4}>
              <Select
                placeholder="Practice"
                style={{ width: '100%' }}
                value={filters.practice}
                onChange={(value) => handleFilterChange('practice', value)}
                allowClear
                showSearch
                optionFilterProp="children"
              >
                {uniquePractices.map(practice => (
                  <Option key={practice} value={practice}>{practice}</Option>
                ))}
              </Select>
            </Col>
            <Col span={4}>
              <Select
                placeholder="Role Type"
                style={{ width: '100%' }}
                value={filters.roleType}
                onChange={(value) => handleFilterChange('roleType', value)}
                allowClear
              >
                {uniqueRoleTypes.map(roleType => (
                  <Option key={roleType} value={roleType}>{roleType}</Option>
                ))}
              </Select>
            </Col>
            <Col span={4}>
              <Select
                placeholder="Status"
                style={{ width: '100%' }}
                value={filters.status}
                onChange={(value) => handleFilterChange('status', value)}
                allowClear
              >
                <Option value="true">Active</Option>
                <Option value="false">Inactive</Option>
              </Select>
            </Col>
            <Col span={4}>
              <Space>
                <Button
                  icon={<FilterOutlined />}
                  onClick={() => {/* Apply filters logic */}}
                >
                  Apply
                </Button>
                <Button
                  icon={<ClearOutlined />}
                  onClick={handleClearFilters}
                >
                  Clear
                </Button>
              </Space>
            </Col>
          </Row>
        </Card>

        {/* Table */}
        <Table
          columns={columns}
          dataSource={affiliations}
          rowKey="affiliationId"
          loading={isLoading}
          scroll={{ x: 1400 }}
          pagination={{
            total: totalCount,
            pageSize: 20,
            showSizeChanger: true,
            showQuickJumper: true,
            showTotal: (total, range) =>
              `${range[0]}-${range[1]} of ${total} affiliations`,
          }}
          rowSelection={rowSelection}
        />
      </Card>
    </div>
  );
};

export default AffiliationList; 