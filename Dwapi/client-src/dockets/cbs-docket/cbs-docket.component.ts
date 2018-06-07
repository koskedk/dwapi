import {Component, OnDestroy, OnInit} from '@angular/core';
import {EmrConfigService} from '../../settings/services/emr-config.service';
import {ConfirmationService, Message} from 'primeng/api';
import {Subscription} from 'rxjs/Subscription';
import {EmrSystem} from '../../settings/model/emr-system';
import {BreadcrumbService} from '../../app/breadcrumb.service';
import {Extract} from '../../settings/model/extract';
import {Docket} from '../../settings/model/docket';
import {CbsService} from '../services/cbs.service';
import {DatabaseProtocol} from '../../settings/model/database-protocol';
import {ExtractPatient} from '../ndwh-docket/model/extract-patient';
import {HubConnection, HubConnectionBuilder, LogLevel} from '@aspnet/signalr';
import {ExtractEvent} from '../../settings/model/extract-event';
import {MasterPatientIndex} from '../models/master-patient-index';

@Component({
  selector: 'liveapp-cbs-docket',
  templateUrl: './cbs-docket.component.html',
  styleUrls: ['./cbs-docket.component.scss']
})
export class CbsDocketComponent implements OnInit, OnDestroy {

    private _hubConnection: HubConnection | undefined;
    public async: any;

    private _confirmationService: ConfirmationService;
    private _emrConfigService: EmrConfigService;

    public getEmr$: Subscription;
    public load$: Subscription;
    public getStatus$: Subscription;
    public get$: Subscription;
    public getCount$: Subscription;
    public emrSystem: EmrSystem;
    public extracts: Extract[];
    public dbProtocol: DatabaseProtocol;
    public extract: Extract;
    public extractPatient: ExtractPatient;
    private extractEvent: ExtractEvent;
    public extractDetails: MasterPatientIndex[] = [];

    public messages: Message[];
    public canLoad: boolean = false;
    public loading: boolean = false;
    public recordCount = 0;
    private sdk: string[] = [];
public colorMappings: any[] = [];
    rowStyleMap: {[key: string]: string};

    public constructor(public breadcrumbService: BreadcrumbService,
                       confirmationService: ConfirmationService, emrConfigService: EmrConfigService, private cbsService: CbsService ) {
        this.breadcrumbService.setItems([
            {label: 'Dockets'},
            {label: 'Case Based Surveillance', routerLink: ['/cbs']}
        ]);
        this._confirmationService = confirmationService;
        this._emrConfigService = emrConfigService;
    }

    public ngOnInit() {
        this.loadData();
        this.liveOnInit();
        this.loadDetails();
    }

    private liveOnInit() {
        this._hubConnection = new HubConnectionBuilder()
            .withUrl(`http://${document.location.hostname}:5757/cbsactivity`)
            .configureLogging(LogLevel.Trace)
            .build();
        this._hubConnection.serverTimeoutInMilliseconds = 120000;

        this._hubConnection.start().catch(err => console.error(err.toString()));

        this._hubConnection.on('ShowCbsProgress', (dwhProgress: any) => {
            if (this.extract) {
                this.extractEvent = {
                    lastStatus: `${dwhProgress.status}`, found: dwhProgress.found, loaded: dwhProgress.loaded,
                    rejected: dwhProgress.rejected, queued: dwhProgress.queued, sent: dwhProgress.sent
                };
                this.extract.extractEvent = {};
                this.extract.extractEvent = this.extractEvent;
                const newWithoutPatientExtract = this.extracts.filter(x => x.name !== 'MasterPatientIndex');
                this.extracts = [...newWithoutPatientExtract, this.extract];
            }
         });
    }

    public loadData(): void {

        this.canLoad = false;

        this.getEmr$ = this._emrConfigService.getDefault()
            .subscribe(
                p => {
                    this.emrSystem = p;
                },
                e => {
                    this.messages = [];
                    this.messages.push({severity: 'error', summary: 'Error Loading data', detail: <any>e});
                },
                () => {

                    if (this.emrSystem) {
                        if (this.emrSystem.extracts) {
                            this.extracts = this.emrSystem.extracts.filter(x => x.docketId === 'CBS');

                            this.extract = this.extracts[0];
                            this.dbProtocol = this.emrSystem.databaseProtocols.find(x => x.id === this.extract.databaseProtocolId);
                            if (this.extract && this.dbProtocol) {
                                this.canLoad = true;
                                this.updateEvent();
                            }
                        }
                    }
                }
            );
    }

    public loadFromEmr(): void {
        this.extractDetails = [];
        this.messages = [];
        this.extractPatient = {extract: this.extract, databaseProtocol: this.dbProtocol};
        this.load$ = this.cbsService.extract(this.extractPatient)
            .subscribe(
                p => {
                    // this.isVerfied = p;
                },
                e => {
                    this.messages = [];
                    this.messages.push({severity: 'error', summary: 'Error loading ', detail: <any>e});
                },
                () => {
                    this.messages = [];
                    this.messages.push({severity: 'success', summary: 'load was successful '});
                    this.updateEvent();
                    this.loadDetails();
                }
            );


    }

    public updateEvent(): void {

        console.log(this.extract);

        if (!this.extract) {
            return;
        }

        this.getCount$ = this.cbsService.getDetailCount()
            .subscribe(
                p => {
                    this.recordCount = p;
                },
                e => {
                    this.messages = [];
                    this.messages.push({severity: 'error', summary: 'Error loading status ', detail: <any>e});
                },
                () => {
                    // console.log(extract);
                }
            );
        this.getStatus$ = this.cbsService.getStatus(this.extract.id)
            .subscribe(
                p => {
                    this.extract.extractEvent = p;
                    // if (this.extract.extractEvent) {
                    //     this.canLoad = this.extract.extractEvent.queued > 0;
                    // }
                },
                e => {
                    this.messages = [];
                    this.messages.push({severity: 'error', summary: 'Error loading status ', detail: <any>e});
                },
                () => {
                    // console.log(extract);
                }
            );

    }
    private isEven(value: number): boolean {
            if ((value % 2) !== 0) {
                return false;
            }
            return true;
    }

    private loadDetails(): void {
        this.loading = true;
        this.get$ = this.cbsService.getDetails()
            .subscribe(
                p => {
                    this.extractDetails = p;
                },
                e => {
                    this.messages = [];
                    this.messages.push({severity: 'error', summary: 'Error Loading data', detail: <any>e});
                },
                () => {
                    this.loading = false;

                    this.sdk = Array.from(new Set(this.extractDetails.map(extract => extract.sxdmPKValueDoB)))
                    this.colorMappings = this.sdk.map((sd, idx) => ({sxdmPKValueDoB: sd, color: this.isEven(idx) ? 'white' : 'pink'}))
                    // this.colorMappings.forEach(value => {
                    //     this.rowStyleMap[value.sxdmPKValueDoB] = value.color;
                    //     console.log(this.rowStyleMap);
                    // });

                }
            );
    }

    lookupRowStyleClass(rowData: MasterPatientIndex) {
       // console.log(rowData);
        return rowData.sxdmPKValueDoB === 'FA343ALPS19730615' ? 'disabled-account-row' : '';

    }

    public ngOnDestroy(): void {
        if (this.getEmr$) {
            this.getEmr$.unsubscribe();
        }
        if (this.getStatus$) {
            this.getStatus$.unsubscribe();
        }
        if (this.load$) {
            this.load$.unsubscribe();
        }
        if (this.get$) {
            this.get$.unsubscribe();
        }
    }
}