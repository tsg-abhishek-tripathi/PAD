import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import {
  Card,
  Row,
  Col,
  Statistic,
  Table,
  Button,
  Space,
  Typography,
  Avatar,
  Tag,
  Progress,
  List,
  Badge,
  Divider,
  Spin,
  Alert,
} from 'antd';
import {
  UserOutlined,
  TeamOutlined,
  TrophyOutlined,
  BankOutlined,
  PlusOutlined,
  EyeOutlined,
  EditOutlined,
  ReloadOutlined,
  TrendingUpOutlined,
  TrendingDownOutlined,
  ClockCircleOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { AppDispatch, RootState } from '../../store/store';
import {
  fetchEmployees,
  selectEmployees,
  selectIsLoading,
  Employee,
} from '../../store/slices/employeeSlice';
import {
  fetchAffiliations,
  selectAffiliations,
  Affiliation,
} from '../../store/slices/affiliationSlice';

const { Title, Text } = Typography;

const Dashboard: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();
  
  const employees = useSelector(selectEmployees);
  const affiliations = useSelector(selectAffiliations);
  const isLoading = useSelector(selectIsLoading);

  const [recentEmployees, setRecentEmployees] = useState<Employee[]>([]);
  const [recentAffiliations, setRecentAffiliations] = useState<Affiliation[]>([]);

  useEffect(() => {
    dispatch(fetchEmployees());
    dispatch(fetchAffiliations());
  }, [dispatch]);

  useEffect(() => {
    // Get recent employees (last 5)
    if (employees.length > 0) {
      setRecentEmployees(employees.slice(0, 5));
    }
    
    // Get recent affiliations (last 5)
    if (affiliations.length > 0) {
      setRecentAffiliations(affiliations.slice(0, 5));
    }
  }, [employees, affiliations]);

  const handleViewAll = (type: string) => {
    navigate(`/${type}`);
  };

  const handleRefresh = () => {
    dispatch(fetchEmployees());
    dispatch(fetchAffiliations());
  };

  // Calculate statistics
  const totalEmployees = employees.length;
  const activeEmployees = employees.filter(emp => emp.isActive).length;
  const totalAffiliations = affiliations.length;
  const activeAffiliations = affiliations.filter(aff => aff.isActive).length;
  const uniquePractices = Array.from(new Set(affiliations.map(aff => aff.practice))).length;
  const uniqueRoleTypes = Array.from(new Set(affiliations.map(aff => aff.roleType))).length;

  // Get level distribution
  const levelDistribution = employees.reduce((acc, emp) => {
    acc[emp.level] = (acc[emp.level] || 0) + 1;
    return acc;
  }, {} as Record<string, number>);

  // Get practice distribution
  const practiceDistribution = affiliations.reduce((acc, aff) => {
    acc[aff.practice] = (acc[aff.practice] || 0) + 1;
    return acc;
  }, {} as Record<string, number>);

  const recentEmployeeColumns: ColumnsType<Employee> = [
    {
      title: 'Employee',
      key: 'employee',
      render: (_, record) => (
        <Space>
          <Avatar 
            size="small" 
            icon={<UserOutlined />}
            src={`https://ui-avatars.com/api/?name=${record.fullName}&size=32&background=1890ff&color=fff`}
          />
          <div>
            <div style={{ fontWeight: 500 }}>{record.fullName}</div>
            <div style={{ fontSize: '12px', color: '#666' }}>{record.employeeCode}</div>
          </div>
        </Space>
      ),
    },
    {
      title: 'Level',
      dataIndex: 'level',
      key: 'level',
      render: (level) => (
        <Tag color={level === 'Partner' ? 'gold' : 'blue'}>{level}</Tag>
      ),
    },
    {
      title: 'Status',
      dataIndex: 'isActive',
      key: 'isActive',
      render: (isActive) => (
        <Badge status={isActive ? 'success' : 'error'} text={isActive ? 'Active' : 'Inactive'} />
      ),
    },
    {
      title: 'Actions',
      key: 'actions',
      render: (_, record) => (
        <Space size="small">
          <Button
            type="text"
            size="small"
            icon={<EyeOutlined />}
            onClick={() => navigate(`/employees/${record.employeeId}`)}
          />
          <Button
            type="text"
            size="small"
            icon={<EditOutlined />}
            onClick={() => navigate(`/employees/${record.employeeId}/edit`)}
          />
        </Space>
      ),
    },
  ];

  const recentAffiliationColumns: ColumnsType<Affiliation> = [
    {
      title: 'Employee',
      key: 'employee',
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
      title: 'Practice',
      dataIndex: 'practice',
      key: 'practice',
      render: (practice) => (
        <Tag color="green">{practice}</Tag>
      ),
    },
    {
      title: 'Role',
      dataIndex: 'roleType',
      key: 'roleType',
      render: (roleType) => (
        <Tag color="blue">{roleType}</Tag>
      ),
    },
    {
      title: 'Status',
      dataIndex: 'isActive',
      key: 'isActive',
      render: (isActive) => (
        <Badge status={isActive ? 'success' : 'error'} text={isActive ? 'Active' : 'Inactive'} />
      ),
    },
  ];

  if (isLoading) {
    return (
      <div style={{ padding: '24px', textAlign: 'center' }}>
        <Spin size="large" />
        <div style={{ marginTop: 16 }}>Loading dashboard...</div>
      </div>
    );
  }

  return (
    <div style={{ padding: '24px' }}>
      {/* Header */}
      <Row gutter={[16, 16]} align="middle" style={{ marginBottom: 24 }}>
        <Col flex="auto">
          <Title level={2} style={{ margin: 0 }}>Dashboard</Title>
          <Text type="secondary">Overview of your Practice Area Affiliation Database</Text>
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

      {/* Key Statistics */}
      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col span={6}>
          <Card>
            <Statistic
              title="Total Employees"
              value={totalEmployees}
              prefix={<UserOutlined />}
              valueStyle={{ color: '#3f8600' }}
            />
            <Progress 
              percent={totalEmployees > 0 ? (activeEmployees / totalEmployees) * 100 : 0} 
              size="small" 
              status="active"
            />
            <Text type="secondary">{activeEmployees} active</Text>
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="Total Affiliations"
              value={totalAffiliations}
              prefix={<TeamOutlined />}
              valueStyle={{ color: '#1890ff' }}
            />
            <Progress 
              percent={totalAffiliations > 0 ? (activeAffiliations / totalAffiliations) * 100 : 0} 
              size="small" 
              status="active"
            />
            <Text type="secondary">{activeAffiliations} active</Text>
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="Unique Practices"
              value={uniquePractices}
              prefix={<TrophyOutlined />}
              valueStyle={{ color: '#722ed1' }}
            />
            <Text type="secondary">Practice areas</Text>
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="Role Types"
              value={uniqueRoleTypes}
              prefix={<BankOutlined />}
              valueStyle={{ color: '#fa8c16' }}
            />
            <Text type="secondary">Different roles</Text>
          </Card>
        </Col>
      </Row>

      <Row gutter={24}>
        {/* Recent Employees */}
        <Col span={12}>
          <Card
            title={
              <Space>
                <UserOutlined />
                Recent Employees
              </Space>
            }
            extra={
              <Button type="link" onClick={() => handleViewAll('employees')}>
                View All
              </Button>
            }
            style={{ marginBottom: 24 }}
          >
            <Table
              columns={recentEmployeeColumns}
              dataSource={recentEmployees}
              rowKey="employeeId"
              pagination={false}
              size="small"
            />
          </Card>

          {/* Level Distribution */}
          <Card title="Employee Level Distribution">
            <List
              dataSource={Object.entries(levelDistribution)}
              renderItem={([level, count]) => (
                <List.Item>
                  <List.Item.Meta
                    title={level}
                    description={`${count} employees`}
                  />
                  <div>
                    <Progress 
                      percent={totalEmployees > 0 ? (count / totalEmployees) * 100 : 0} 
                      size="small" 
                      format={() => `${count}`}
                    />
                  </div>
                </List.Item>
              )}
            />
          </Card>
        </Col>

        {/* Recent Affiliations */}
        <Col span={12}>
          <Card
            title={
              <Space>
                <TeamOutlined />
                Recent Affiliations
              </Space>
            }
            extra={
              <Button type="link" onClick={() => handleViewAll('affiliations')}>
                View All
              </Button>
            }
            style={{ marginBottom: 24 }}
          >
            <Table
              columns={recentAffiliationColumns}
              dataSource={recentAffiliations}
              rowKey="affiliationId"
              pagination={false}
              size="small"
            />
          </Card>

          {/* Practice Distribution */}
          <Card title="Practice Area Distribution">
            <List
              dataSource={Object.entries(practiceDistribution)}
              renderItem={([practice, count]) => (
                <List.Item>
                  <List.Item.Meta
                    title={practice}
                    description={`${count} affiliations`}
                  />
                  <div>
                    <Progress 
                      percent={totalAffiliations > 0 ? (count / totalAffiliations) * 100 : 0} 
                      size="small" 
                      format={() => `${count}`}
                    />
                  </div>
                </List.Item>
              )}
            />
          </Card>
        </Col>
      </Row>

      {/* Quick Actions */}
      <Card title="Quick Actions" style={{ marginTop: 24 }}>
        <Row gutter={[16, 16]}>
          <Col span={6}>
            <Button 
              type="primary" 
              block 
              icon={<PlusOutlined />}
              onClick={() => navigate('/employees/new')}
            >
              Add Employee
            </Button>
          </Col>
          <Col span={6}>
            <Button 
              block 
              icon={<TeamOutlined />}
              onClick={() => navigate('/affiliations/new')}
            >
              Add Affiliation
            </Button>
          </Col>
          <Col span={6}>
            <Button 
              block 
              icon={<EyeOutlined />}
              onClick={() => navigate('/employees')}
            >
              View Employees
            </Button>
          </Col>
          <Col span={6}>
            <Button 
              block 
              icon={<TrophyOutlined />}
              onClick={() => navigate('/reports')}
            >
              View Reports
            </Button>
          </Col>
        </Row>
      </Card>
    </div>
  );
};

export default Dashboard; 