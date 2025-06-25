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
  Divider,
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
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { AppDispatch, RootState } from '../../store/store';
import {
  fetchEmployees,
  deleteEmployee,
  searchEmployees,
  setFilters,
  clearFilters,
  selectEmployees,
  selectIsLoading,
  selectError,
  selectTotalCount,
  selectFilters,
  Employee,
} from '../../store/slices/employeeSlice';

const { Search } = Input;
const { Option } = Select;

const EmployeeList: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();
  
  const employees = useSelector(selectEmployees);
  const isLoading = useSelector(selectIsLoading);
  const error = useSelector(selectError);
  const totalCount = useSelector(selectTotalCount);
  const filters = useSelector(selectFilters);

  const [searchValue, setSearchValue] = useState(filters.search);
  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);

  useEffect(() => {
    dispatch(fetchEmployees());
  }, [dispatch]);

  useEffect(() => {
    if (error) {
      message.error(error);
    }
  }, [error]);

  const handleSearch = (value: string) => {
    setSearchValue(value);
    if (value.trim()) {
      dispatch(searchEmployees(value));
    } else {
      dispatch(fetchEmployees());
    }
    dispatch(setFilters({ search: value }));
  };

  const handleFilterChange = (key: string, value: string) => {
    dispatch(setFilters({ [key]: value }));
  };

  const handleClearFilters = () => {
    setSearchValue('');
    dispatch(clearFilters());
    dispatch(fetchEmployees());
  };

  const handleDelete = async (employeeId: number) => {
    try {
      await dispatch(deleteEmployee(employeeId)).unwrap();
      message.success('Employee deleted successfully');
    } catch (error) {
      message.error('Failed to delete employee');
    }
  };

  const handleRefresh = () => {
    dispatch(fetchEmployees());
  };

  const columns: ColumnsType<Employee> = [
    {
      title: 'Employee Code',
      dataIndex: 'employeeCode',
      key: 'employeeCode',
      width: 120,
      fixed: 'left',
    },
    {
      title: 'Name',
      dataIndex: 'fullName',
      key: 'fullName',
      width: 200,
      fixed: 'left',
      render: (text, record) => (
        <div>
          <div style={{ fontWeight: 500 }}>{text}</div>
          <div style={{ fontSize: '12px', color: '#666' }}>{record.email}</div>
        </div>
      ),
    },
    {
      title: 'Title',
      dataIndex: 'title',
      key: 'title',
      width: 150,
      ellipsis: true,
    },
    {
      title: 'Level',
      dataIndex: 'level',
      key: 'level',
      width: 100,
      render: (level) => (
        <Tag color={level === 'Partner' ? 'gold' : level === 'Associate' ? 'blue' : 'green'}>
          {level}
        </Tag>
      ),
    },
    {
      title: 'Home Office',
      dataIndex: 'homeOffice',
      key: 'homeOffice',
      width: 150,
      render: (homeOffice) => homeOffice?.officeName || '-',
    },
    {
      title: 'Affiliations',
      key: 'affiliations',
      width: 120,
      render: (_, record) => (
        <Badge count={record.affiliations?.length || 0} showZero>
          <Tag color="purple">Affiliations</Tag>
        </Badge>
      ),
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
      title: 'Hire Date',
      dataIndex: 'hireDate',
      key: 'hireDate',
      width: 120,
      render: (date) => date ? new Date(date).toLocaleDateString() : '-',
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
              onClick={() => navigate(`/employees/${record.employeeId}`)}
            />
          </Tooltip>
          <Tooltip title="Edit">
            <Button
              type="text"
              icon={<EditOutlined />}
              onClick={() => navigate(`/employees/${record.employeeId}/edit`)}
            />
          </Tooltip>
          <Tooltip title="Delete">
            <Popconfirm
              title="Are you sure you want to delete this employee?"
              onConfirm={() => handleDelete(record.employeeId)}
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

  return (
    <div style={{ padding: '24px' }}>
      <Card>
        <Row gutter={[16, 16]} align="middle" style={{ marginBottom: 24 }}>
          <Col flex="auto">
            <h2 style={{ margin: 0 }}>Employees</h2>
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
                onClick={() => navigate('/employees/new')}
              >
                Add Employee
              </Button>
            </Space>
          </Col>
        </Row>

        {/* Statistics */}
        <Row gutter={16} style={{ marginBottom: 24 }}>
          <Col span={6}>
            <Card>
              <Statistic
                title="Total Employees"
                value={totalCount}
                valueStyle={{ color: '#3f8600' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Active Employees"
                value={employees.filter(emp => emp.isActive).length}
                valueStyle={{ color: '#1890ff' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Partners"
                value={employees.filter(emp => emp.level === 'Partner').length}
                valueStyle={{ color: '#722ed1' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Associates"
                value={employees.filter(emp => emp.level === 'Associate').length}
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
                placeholder="Search employees..."
                value={searchValue}
                onChange={(e) => setSearchValue(e.target.value)}
                onSearch={handleSearch}
                enterButton={<SearchOutlined />}
                allowClear
              />
            </Col>
            <Col span={4}>
              <Select
                placeholder="Level"
                style={{ width: '100%' }}
                value={filters.level}
                onChange={(value) => handleFilterChange('level', value)}
                allowClear
              >
                <Option value="Partner">Partner</Option>
                <Option value="Associate">Associate</Option>
                <Option value="Staff">Staff</Option>
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
              <Select
                placeholder="Office"
                style={{ width: '100%' }}
                value={filters.office}
                onChange={(value) => handleFilterChange('office', value)}
                allowClear
              >
                {Array.from(new Set(employees.map(emp => emp.homeOffice?.officeName).filter(Boolean))).map(office => (
                  <Option key={office} value={office}>{office}</Option>
                ))}
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
          dataSource={employees}
          rowKey="employeeId"
          loading={isLoading}
          scroll={{ x: 1200 }}
          pagination={{
            total: totalCount,
            pageSize: 20,
            showSizeChanger: true,
            showQuickJumper: true,
            showTotal: (total, range) =>
              `${range[0]}-${range[1]} of ${total} employees`,
          }}
          rowSelection={rowSelection}
        />
      </Card>
    </div>
  );
};

export default EmployeeList; 