import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTypographyModule } from 'ng-zorro-antd/typography';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzCarouselModule } from 'ng-zorro-antd/carousel';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzBadgeModule } from 'ng-zorro-antd/badge';

interface Feature {
  icon: string;
  title: string;
  description: string;
  color: string;
}

interface Stat {
  value: string;
  label: string;
  prefix?: string;
  suffix?: string;
}

interface TestType {
  icon: string;
  name: string;
  description: string;
}

interface HowItWorksStep {
  icon: string;
  title: string;
  description: string;
  forRole: 'candidate' | 'employer';
}

interface Testimonial {
  name: string;
  role: string;
  company: string;
  avatar: string;
  rating: number;
  comment: string;
}

interface RecentVacancy {
  id: number;
  title: string;
  company: string;
  location: string;
  type: string;
  salary: string;
  postedDate: string;
  applicants: number;
}

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    NzCardModule,
    NzGridModule,
    NzIconModule,
    NzButtonModule,
    NzTypographyModule,
    NzStatisticModule,
    NzCarouselModule,
    NzTagModule,
    NzBadgeModule,
  ],
  templateUrl: './landing.html',
  styleUrls: ['./landing.scss'],
})
export class Landing {
  stats: Stat[] = [
    { value: '10000', label: 'Active Candidates', suffix: '+' },
    { value: '500', label: 'Organizations', suffix: '+' },
    { value: '50', label: 'Time Saved', suffix: '%' },
    { value: '5000', label: 'Tests Completed', suffix: '+' },
  ];

  features: Feature[] = [
    {
      icon: 'file-search',
      title: 'Smart Job Matching',
      description:
        'Advanced algorithms match candidates with perfect opportunities based on skills and experience.',
      color: '#1890ff',
    },
    {
      icon: 'experiment',
      title: '6-Type Testing System',
      description:
        'Multiple choice, essays, coding challenges, file uploads, and more for comprehensive evaluation.',
      color: '#52c41a',
    },
    {
      icon: 'bar-chart',
      title: 'Real-time Analytics',
      description:
        'Track application performance, test results, and hiring metrics with powerful dashboards.',
      color: '#faad14',
    },
    {
      icon: 'safety-certificate',
      title: 'Secure & Transparent',
      description:
        'Complete audit trails, GDPR compliance, and fair evaluation for every candidate.',
      color: '#722ed1',
    },
    {
      icon: 'message',
      title: 'Built-in Communication',
      description: 'Direct messaging between candidates and employers with notification system.',
      color: '#13c2c2',
    },
    {
      icon: 'thunderbolt',
      title: 'Quick Deployment',
      description:
        'Post vacancies and start receiving applications within minutes. No complex setup required.',
      color: '#eb2f96',
    },
  ];

  testTypes: TestType[] = [
    {
      icon: 'check-square',
      name: 'Multiple Choice',
      description: 'Auto-graded objective questions',
    },
    { icon: 'check-circle', name: 'True/False', description: 'Quick knowledge verification' },
    { icon: 'edit', name: 'Short Answer', description: 'Brief written responses' },
    { icon: 'file-text', name: 'Essay', description: 'In-depth written evaluation' },
    { icon: 'code', name: 'Coding', description: 'Technical programming tasks' },
    { icon: 'upload', name: 'File Upload', description: 'Portfolio or document submission' },
  ];

  candidateSteps: HowItWorksStep[] = [
    {
      icon: 'user-add',
      title: 'Create Your Profile',
      description:
        'Sign up and build a comprehensive profile with your education, experience, and skills.',
      forRole: 'candidate',
    },
    {
      icon: 'search',
      title: 'Browse & Apply',
      description:
        'Search thousands of opportunities and apply with one click. Track all applications in your dashboard.',
      forRole: 'candidate',
    },
    {
      icon: 'file-protect',
      title: 'Take Assessments',
      description: 'Complete professional tests designed by employers to showcase your abilities.',
      forRole: 'candidate',
    },
    {
      icon: 'trophy',
      title: 'Get Hired',
      description: 'Receive offers from top organizations and start your dream career.',
      forRole: 'candidate',
    },
  ];

  employerSteps: HowItWorksStep[] = [
    {
      icon: 'bank',
      title: 'Register Organization',
      description: 'Create your organization profile and add team members with HR manager roles.',
      forRole: 'employer',
    },
    {
      icon: 'file-add',
      title: 'Post Vacancies',
      description:
        'Create detailed job listings with requirements, responsibilities, and compensation.',
      forRole: 'employer',
    },
    {
      icon: 'audit',
      title: 'Review & Test',
      description:
        'Screen applications and assign custom tests to evaluate candidate capabilities.',
      forRole: 'employer',
    },
    {
      icon: 'team',
      title: 'Hire Top Talent',
      description: 'Make data-driven hiring decisions with objective test results and analytics.',
      forRole: 'employer',
    },
  ];

  testimonials: Testimonial[] = [
    {
      name: 'Nino Beridze',
      role: 'HR Director',
      company: 'Tech Solutions Georgia',
      avatar: 'N',
      rating: 5,
      comment:
        'TalentBridge reduced our hiring time by 60%. The testing system is exceptional and helps us find the best candidates quickly.',
    },
    {
      name: 'Giorgi Kapanadze',
      role: 'Software Developer',
      company: 'Hired through TalentBridge',
      avatar: 'G',
      rating: 5,
      comment:
        'Found my dream job in just 2 weeks! The platform made applying easy and the test process was fair and transparent.',
    },
    {
      name: 'Mariam Tsereteli',
      role: 'School Principal',
      company: 'International School Tbilisi',
      avatar: 'M',
      rating: 5,
      comment:
        'We hired 15 teachers using TalentBridge. The education-specific tests helped us identify qualified candidates efficiently.',
    },
  ];

  recentVacancies: RecentVacancy[] = [
    {
      id: 1,
      title: 'Senior Full Stack Developer',
      company: 'TBC Bank',
      location: 'Tbilisi, Georgia',
      type: 'Full-time',
      salary: '3000-5000 GEL',
      postedDate: '2 days ago',
      applicants: 45,
    },
    {
      id: 2,
      title: 'Marketing Manager',
      company: 'Bank of Georgia',
      location: 'Tbilisi, Georgia',
      type: 'Full-time',
      salary: '2500-4000 GEL',
      postedDate: '1 day ago',
      applicants: 32,
    },
    {
      id: 3,
      title: 'English Teacher',
      company: 'British-Georgian Academy',
      location: 'Batumi, Georgia',
      type: 'Full-time',
      salary: '1500-2500 GEL',
      postedDate: '3 days ago',
      applicants: 28,
    },
    {
      id: 4,
      title: 'Nurse',
      company: 'Tbilisi Central Hospital',
      location: 'Tbilisi, Georgia',
      type: 'Full-time',
      salary: '1200-2000 GEL',
      postedDate: '1 week ago',
      applicants: 67,
    },
  ];

  activeTab: 'candidate' | 'employer' = 'candidate';

  scrollToSection(sectionId: string): void {
    const element = document.getElementById(sectionId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  switchTab(tab: 'candidate' | 'employer'): void {
    this.activeTab = tab;
  }
}
