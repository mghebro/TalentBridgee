import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTypographyModule } from 'ng-zorro-antd/typography';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';

interface ValueCard {
  icon: string;
  title: string;
  description: string;
}

interface Stat {
  number: string;
  label: string;
  subtext: string;
}

@Component({
  selector: 'app-about-us',
  standalone: true,
  imports: [
    CommonModule,
    NzCardModule,
    NzGridModule,
    NzIconModule,
    NzButtonModule,
    NzTypographyModule,
    NzStatisticModule,
  ],
  templateUrl: './about.html',
  styleUrls: ['./about.scss'],
})
export class About {
  values: ValueCard[] = [
    {
      icon: 'check-circle',
      title: 'Objectivity',
      description: 'Fair, unbiased candidate evaluation through standardized testing',
    },
    {
      icon: 'team',
      title: 'Inclusivity',
      description: 'Equal opportunities across all industries and backgrounds',
    },
    {
      icon: 'trophy',
      title: 'Excellence',
      description: 'Commitment to the highest standards in recruitment technology',
    },
    {
      icon: 'rise',
      title: 'Innovation',
      description: 'Continuously evolving with AI and modern hiring practices',
    },
  ];

  stats: Stat[] = [
    { number: '6', label: 'Question Types', subtext: 'Comprehensive Testing' },
    { number: '50%', label: 'Time Saved', subtext: 'Faster Hiring' },
    { number: '40%', label: 'Cost Reduced', subtext: 'Efficient Process' },
    { number: '5', label: 'Industries', subtext: 'Universal Platform' },
  ];
}
